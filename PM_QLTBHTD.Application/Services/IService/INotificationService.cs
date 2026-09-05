using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    /// <summary>Đẩy sự kiện realtime xuống client (SignalR). Interface đặt ở Application để
    /// PhieuKiemTraService có thể inject; implementation thật (dùng IHubContext) đặt ở API project
    /// vì Hub gắn với hạ tầng web, tránh Application phải reference ngược lên API.</summary>
    public interface INotificationService
    {
        Task ThongBaoPhieuMoiAsync(PhieuKiemTraDto phieu);
    }
}
