using IRepositories.Entity.Inventory;

namespace IRepositories.IRepository.Inventory
{
    public interface IVaccineCenterRepository : IGenericRepository<VaccineCenter>
    {
        Task<VaccineCenter?> GetByNameAsync(string name);
    }
}
