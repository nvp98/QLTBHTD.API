using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Tính Diem_Si_DatDuoc cho tất cả chỉ tiêu lá của một phiếu,
    /// ghi kết quả vào ChiTietKiemTra trước khi ScoringEngine được gọi.
    /// </summary>
    public interface IChiTieuScoringService
    {
        /// <summary>
        /// Nhận batch dữ liệu nhập từ form, tính Si cho từng chỉ tiêu và lưu vào DB.
        /// Gọi trước TinhDiemNhomAsync.
        /// </summary>
        Task TinhVaLuuDiemSiAsync(int idPhieu, IEnumerable<NhapChiTietKiemTraDto> danhSachNhap, CancellationToken ct = default);

        /// <summary>
        /// Tính lại Sᵢ + khuyến cáo hành động cho TẤT CẢ chỉ tiêu đã có dữ liệu trong 1 phiếu, dùng dữ
        /// liệu đã lưu (GiaTriNhap_So + ChiTietKiemTra_Input) — không cần người dùng nhập lại. Dùng khi
        /// sửa cấu hình (Rule/Formula/Ngưỡng/nguồn Input) sau khi phiếu đã tồn tại, để Sᵢ/khuyến cáo
        /// phản ánh đúng cấu hình mới mà không phải "Tính lại CSSK" (chỉ gộp lại từ Sᵢ cũ, không tính lại Sᵢ).
        /// </summary>
        Task TinhLaiToanBoSiAsync(int idPhieu, CancellationToken ct = default);

        /// <summary>
        /// Tính điểm cho 1 chỉ tiêu kiểu LoaiTinhDiem='LF' — tự gom tối đa 12 tháng
        /// (giá trị tháng này + các phiếu trước đó của cùng thiết bị) rồi tra ngưỡng.
        /// Dùng chung cho cả luồng nhập liệu (ChiTieuScoringService) và luồng tạo phiếu (PhieuKiemTraService).
        /// </summary>
        Task<(decimal? Lf, decimal? DiemSi, string? HanhDong)> TinhDiemLFAsync(
            int idPhieu, int idThietBi, DateTime ngayKiemTra, int idChiTieu, decimal? giaTriThangNay,
            CancellationToken ct = default);
    }
}
