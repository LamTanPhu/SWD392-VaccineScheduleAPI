using IRepositories.Entity.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Schedules
{
    public interface IVaccineScheduleRepository: IGenericRepository<VaccinationSchedule>
    {
        Task<bool> ExistsAsync(string profileId, string orderVaccineDetailsId, string orderPackageDetailsId, int doseNumber);

    }
}
