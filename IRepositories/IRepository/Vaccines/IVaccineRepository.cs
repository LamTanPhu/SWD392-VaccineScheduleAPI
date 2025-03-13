using IRepositories.Entity.Vaccines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Vaccines
{
    public interface IVaccineRepository : IGenericRepository<Vaccine>
    {
    }
}
