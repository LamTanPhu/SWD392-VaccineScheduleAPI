using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Schedules
{
    public class VaccineScheduleRepository: GenericRepository<VaccinationSchedule>, IVaccineScheduleRepository
    {
        public VaccineScheduleRepository(DatabaseContext context) : base(context) { }
        public async Task<bool> ExistsAsync(string profileId, string orderVaccineDetailsId, string orderPackageDetailsId, int doseNumber)
        {
            return await _context.VaccinationSchedules
                .AnyAsync(s => s.ProfileId == profileId &&
                               s.DoseNumber == doseNumber &&
                               s.Status != 0 && // Ensure the schedule is active
                               ((orderVaccineDetailsId != null && s.OrderVaccineDetailsId == orderVaccineDetailsId) ||
                                (orderPackageDetailsId != null && s.OrderPackageDetailsId == orderPackageDetailsId)));
        }

    }
}
