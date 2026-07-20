using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Engine tính điểm nhóm chỉ tiêu theo cây đệ quy.
    /// Đọc cache CBM_KetQuaNhom trước — tính lại chỉ khi chưa có cache.
    /// Ném exception rõ ràng thay vì trả giá trị mặc định khi thiếu dữ liệu/cấu hình.
    /// </summary>
    public interface IScoringEngine
    {
        /// <summary>
        /// Tính điểm một nhóm chỉ tiêu cho phiếu kiểm tra.
        /// Đệ quy qua các nhóm con (NHOM_CON) và đọc điểm Si từ ChiTietKiemTra (CHITIEU).
        /// </summary>
        Task<decimal> TinhDiemNhomAsync(int idNhomChiTieu, int idPhieu, CancellationToken ct = default);

        /// <summary>
        /// Tính điểm toàn bộ cây từ nhóm gốc trở xuống cho một phiếu.
        /// Trả về danh sách kết quả từng nhóm (đã ghi cache).
        /// </summary>
        Task<IReadOnlyList<KetQuaNhomDto>> TinhDiemCayAsync(int idLoaiThietBi, int idPhieu, CancellationToken ct = default);

        /// <summary>
        /// Tính điểm CSSK đại diện cho cả phiếu và ghi vào PhieuKiemTra.TongDiem_Soqt + CapDoCanhBao.
        /// Nhóm dùng để tính: ID_NhomChiTieu của phiếu nếu có, ngược lại tự tìm nhóm gốc
        /// (ID_NhomCha=null) của loại thiết bị — ném NhieuNhomGocException nếu có nhiều hơn 1 nhóm gốc.
        /// Trả null (không ném lỗi) khi chưa đủ dữ liệu/công thức để tính — coi là "chưa tính được", không phải lỗi.
        /// </summary>
        Task<decimal?> TinhVaLuuTongDiemAsync(int idPhieu, CancellationToken ct = default);
    }
}
