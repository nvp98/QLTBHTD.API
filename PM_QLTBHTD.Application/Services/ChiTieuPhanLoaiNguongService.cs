using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuPhanLoaiNguongService : IChiTieuPhanLoaiNguongService
    {
        private readonly IPhanLoaiNguongRepository _repository;
        private readonly IKetQuaPhanLoaiThangRepository _ketQuaRepository;

        public ChiTieuPhanLoaiNguongService(
            IPhanLoaiNguongRepository repository,
            IKetQuaPhanLoaiThangRepository ketQuaRepository)
        {
            _repository = repository;
            _ketQuaRepository = ketQuaRepository;
        }

        private static ChiTieuPhanLoaiNguongDto ToDto(CBM_ChiTieu_PhanLoaiNguong e) => new()
        {
            ID_PhanLoai = e.ID_PhanLoai,
            ID_ChiTieu = e.ID_ChiTieu,
            MaMuc = e.MaMuc,
            GiaTriTu = e.GiaTriTu,
            GiaTriDen = e.GiaTriDen,
            GiaTriTu_BaoGom = e.GiaTriTu_BaoGom,
            GiaTriDen_BaoGom = e.GiaTriDen_BaoGom,
            TrongSo = e.TrongSo,
            ThuTu = e.ThuTu,
        };

        private static KetQuaPhanLoaiThangDto ToKetQuaDto(CBM_KetQuaPhanLoaiThang e) => new()
        {
            IDPhieu = e.IDPhieu,
            ID_ChiTieu = e.ID_ChiTieu,
            Nam = e.Nam,
            Thang = e.Thang,
            GiaTriDo = e.GiaTriDo,
            MaMuc = e.MaMuc,
            TrongSo = e.TrongSo,
        };

        public async Task<IEnumerable<ChiTieuPhanLoaiNguongDto>> GetByChiTieuAsync(int idChiTieu)
            => (await _repository.GetByChiTieuAsync(idChiTieu)).Select(ToDto);

        public async Task<ChiTieuPhanLoaiNguongDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : ToDto(e);
        }

        public async Task<ChiTieuPhanLoaiNguongDto> CreateAsync(CreateChiTieuPhanLoaiNguongDto dto)
        {
            var entity = new CBM_ChiTieu_PhanLoaiNguong
            {
                ID_ChiTieu = dto.ID_ChiTieu,
                MaMuc = dto.MaMuc,
                GiaTriTu = dto.GiaTriTu,
                GiaTriDen = dto.GiaTriDen,
                GiaTriTu_BaoGom = dto.GiaTriTu_BaoGom,
                GiaTriDen_BaoGom = dto.GiaTriDen_BaoGom,
                TrongSo = dto.TrongSo,
                ThuTu = dto.ThuTu,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuPhanLoaiNguongDto?> UpdateAsync(int id, UpdateChiTieuPhanLoaiNguongDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.MaMuc = dto.MaMuc;
            entity.GiaTriTu = dto.GiaTriTu;
            entity.GiaTriDen = dto.GiaTriDen;
            entity.GiaTriTu_BaoGom = dto.GiaTriTu_BaoGom;
            entity.GiaTriDen_BaoGom = dto.GiaTriDen_BaoGom;
            entity.TrongSo = dto.TrongSo;
            entity.ThuTu = dto.ThuTu;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<KetQuaPhanLoaiThangDto>> GetKetQuaThangAsync(int idPhieu, int idChiTieu)
            => (await _ketQuaRepository.GetByPhieuChiTieuAsync(idPhieu, idChiTieu)).Select(ToKetQuaDto);
    }
}
