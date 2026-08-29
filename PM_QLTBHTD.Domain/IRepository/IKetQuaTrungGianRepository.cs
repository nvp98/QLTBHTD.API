using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    /// <summary>
    /// Ghi APPEND-only (không upsert, không cần GetById theo PK long) — đọc lại dùng
    /// IAppDbContext.KetQuaTrungGians (LINQ) như các bảng khác, không qua repository này.
    /// </summary>
    public interface IKetQuaTrungGianRepository
    {
        Task AddRangeAsync(IEnumerable<CBM_KetQuaTrungGian> items);
        Task<int> SaveChangesAsync();
    }
}
