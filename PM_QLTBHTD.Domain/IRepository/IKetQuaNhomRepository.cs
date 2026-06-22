using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IKetQuaNhomRepository : IGenericRepository<CBM_KetQuaNhom>
    {
        Task<CBM_KetQuaNhom?> GetByPhieuNhomAsync(int idPhieu, int idNhomChiTieu);
        Task<IEnumerable<CBM_KetQuaNhom>> GetByPhieuAsync(int idPhieu);
        Task UpsertAsync(CBM_KetQuaNhom ketQua);
    }
}
