using IRepositories.Entity;

namespace IRepositories.IRepository
{
    public interface IVaccineCenterRepository : IGenericRepository<VaccineCenter>
    {
        Task<VaccineCenter?> GetByNameAsync(string name);
    }
}
