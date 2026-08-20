using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface ILichSuService
    {
        /// <summary>Lịch sử dữ liệu đo + kết quả tính toán của 1 thiết bị, theo 1 nhóm chỉ tiêu,
        /// qua tất cả các phiếu kiểm tra trước đây — mỗi hàng là 1 phiếu, mỗi cột là 1 chỉ tiêu
        /// trong nhóm (giá trị đo + Sᵢ), kèm điểm của chính nhóm đó nếu đã tính.</summary>
        Task<LichSuNhomDto?> GetLichSuAsync(int idThietBi, int idNhomChiTieu);
    }
}
