using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NguongScoringService : INguongScoringService
    {
        private readonly INguongRepository _nguongRepo;

        public NguongScoringService(INguongRepository nguongRepo)
        {
            _nguongRepo = nguongRepo;
        }

        /// <summary>
        /// Lấy danh sách ngưỡng theo chỉ tiêu, duyệt từ điểm cao xuống thấp,
        /// trả về Diem_Si của ngưỡng đầu tiên thỏa mãn điều kiện, hoặc null nếu không khớp.
        /// </summary>
        public async Task<decimal?> TinhDiemAsync(
            int idChiTieu,
            decimal? giaTriDon,
            Dictionary<string, decimal> tatCaGiaTri)
        {
            var nguongs = await _nguongRepo.GetByChiTieuAsync(idChiTieu);

            foreach (var ng in nguongs.OrderByDescending(x => x.Diem_Si))
            {
                bool matched = !string.IsNullOrEmpty(ng.BieuThuc_Logic)
                    ? NguongEvaluator.EvalNCalc(ng.BieuThuc_Logic, tatCaGiaTri)
                    : NguongEvaluator.KiemTraRange(giaTriDon, ng);

                if (matched)
                    return ng.Diem_Si;
            }

            return null;
        }
    }
}
