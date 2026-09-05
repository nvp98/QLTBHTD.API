using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PM_QLTBHTD.API.Hubs
{
    /// <summary>Hub chỉ dùng để server đẩy (push) sự kiện xuống client — không có method nào cho
    /// client gọi ngược lại. Xác thực bằng JWT hiện có (token đọc từ query string access_token,
    /// xem cấu hình JwtBearerEvents.OnMessageReceived trong Program.cs).</summary>
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
