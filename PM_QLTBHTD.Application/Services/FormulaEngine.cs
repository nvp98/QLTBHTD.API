using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Tầng tính toán trung gian giữa Input và Threshold/Rule (xem CBM_ChiTieu_Formula).
    /// </summary>
    public class FormulaEngine : IFormulaEngine
    {
        private readonly IChiTieuFormulaRepository _formulaRepo;
        private readonly IChiTieuFormulaThamSoRepository _thamSoRepo;
        private readonly IFormulaFunctionRegistry _functions;
        private readonly IAppDbContext _db;

        public FormulaEngine(
            IChiTieuFormulaRepository formulaRepo,
            IChiTieuFormulaThamSoRepository thamSoRepo,
            IFormulaFunctionRegistry functions,
            IAppDbContext db)
        {
            _formulaRepo = formulaRepo;
            _thamSoRepo = thamSoRepo;
            _functions = functions;
            _db = db;
        }

        public async Task<Dictionary<string, decimal>> EvaluateAllAsync(
            int idChiTieu,
            int idPhieu,
            int idThietBi,
            Dictionary<string, decimal> inputValues,
            CancellationToken ct = default)
        {
            var formulas = (await _formulaRepo.GetByChiTieuAsync(idChiTieu))
                .OrderBy(f => f.ThuTu)
                .ToList();

            var ketQua = new Dictionary<string, decimal>();
            var maKetQuaTheoId = formulas.ToDictionary(f => f.ID_Formula, f => f.MaKetQua);

            foreach (var f in formulas)
            {
                var thamSoList = await _thamSoRepo.GetByFormulaAsync(f.ID_Formula);
                var vars = new Dictionary<string, decimal>();

                foreach (var ts in thamSoList)
                {
                    vars[ts.MaThamSo] = await ResolveThamSoAsync(f, ts, idPhieu, idThietBi, inputValues, ketQua, maKetQuaTheoId, ct);
                }

                decimal giaTri;
                try
                {
                    giaTri = f.LoaiFormula switch
                    {
                        "NCALC"    => NguongEvaluator.EvalNCalcNumeric(f.BieuThuc ?? string.Empty, vars),
                        "FUNCTION" => _functions.Invoke(f.TenFunction ?? string.Empty, vars),
                        _ => throw new InvalidOperationException($"LoaiFormula '{f.LoaiFormula}' không hỗ trợ.")
                    };
                }
                catch (Exception ex) when (ex is not LoiFormulaException)
                {
                    throw new LoiFormulaException(idChiTieu, f.MaKetQua, ex.Message);
                }

                ketQua[f.MaKetQua] = giaTri;
            }

            return ketQua;
        }

        private async Task<decimal> ResolveThamSoAsync(
            CBM_ChiTieu_Formula formula,
            CBM_ChiTieu_Formula_ThamSo ts,
            int idPhieu,
            int idThietBi,
            Dictionary<string, decimal> inputValues,
            Dictionary<string, decimal> ketQuaDaTinh,
            Dictionary<int, string> maKetQuaTheoId,
            CancellationToken ct)
        {
            switch (ts.NguonGiaTri)
            {
                case "INPUT":
                    if (ts.MaInput == null || !inputValues.TryGetValue(ts.MaInput, out var vInput))
                        throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                            $"Thiếu giá trị Input '{ts.MaInput}'.");
                    return vInput;

                case "FORMULA_KETQUA":
                    if (ts.ID_FormulaNguon is null || !maKetQuaTheoId.TryGetValue(ts.ID_FormulaNguon.Value, out var maKq)
                        || !ketQuaDaTinh.TryGetValue(maKq, out var vFormula))
                        throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                            $"Formula nguồn ID={ts.ID_FormulaNguon} chưa được evaluate — kiểm tra lại ThuTu (phải nhỏ hơn Formula hiện tại).");
                    return vFormula;

                case "HANGSO":
                    return ts.GiaTriHangSo
                        ?? throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                            $"Tham số '{ts.MaThamSo}' khai NguonGiaTri=HANGSO nhưng GiaTriHangSo rỗng.");

                case "CHITIEU_SI":
                    var si = await _db.ChiTietKiemTras
                        .Where(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == ts.ID_ChiTieuNguon)
                        .Select(x => x.Diem_Si_DatDuoc)
                        .FirstOrDefaultAsync(ct);
                    return si ?? throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                        $"Chỉ tiêu nguồn ID={ts.ID_ChiTieuNguon} chưa có Si trong phiếu {idPhieu}.");

                case "THIETBI_THUOCTINH":
                    return await ResolveThuocTinhThietBiAsync(formula, ts, idThietBi, ct);

                default:
                    throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                        $"NguonGiaTri '{ts.NguonGiaTri}' không hỗ trợ.");
            }
        }

        private async Task<decimal> ResolveThuocTinhThietBiAsync(
            CBM_ChiTieu_Formula formula, CBM_ChiTieu_Formula_ThamSo ts, int idThietBi, CancellationToken ct)
        {
            var thietBi = await _db.ThietBis.FirstOrDefaultAsync(t => t.ID_ThietBi == idThietBi, ct)
                ?? throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua, $"Không tìm thấy Thiết bị ID={idThietBi}.");

            return ts.TenThuocTinhTB switch
            {
                "TaiDinhMuc" => thietBi.TaiDinhMuc
                    ?? throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua, "Thiết bị chưa cấu hình TaiDinhMuc."),
                _ => throw new LoiFormulaException(formula.ID_ChiTieu, formula.MaKetQua,
                    $"Thuộc tính Thiết bị '{ts.TenThuocTinhTB}' không hỗ trợ.")
            };
        }
    }
}
