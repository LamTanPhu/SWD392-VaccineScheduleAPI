using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repositories.Context
{
    public interface IDbContext : IDisposable
    {
        DbSet<T> Set<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}