using Microsoft.AspNetCore.SignalR;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Hubs
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public Task ThongBaoPhieuMoiAsync(PhieuKiemTraDto phieu) =>
            _hub.Clients.All.SendAsync("PhieuKiemTraCreated", new
            {
                phieu.ID_Phieu,
                phieu.ID_ThietBi,
                phieu.TenThietBi,
                phieu.TenTram,
                phieu.NgayKiemTra,
                phieu.TongDiem_Soqt,
                phieu.CapDoCanhBao,
            });
    }
}
