using IRepositories.Entity.Vaccines;
using IRepositories.IRepository.Vaccines;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Vaccines
{
    public class VaccineCategoryRepository : GenericRepository<VaccineCategory>, IVaccineCategoryRepository
    {
        public VaccineCategoryRepository(DatabaseContext context) : base(context) { }
    }
}
