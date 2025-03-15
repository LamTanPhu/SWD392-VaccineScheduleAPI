
﻿using IRepositories.Entity.Schedules;
using System;

﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Schedules
{

    public interface IVaccineHistoryRepository : IGenericRepository<VaccineHistory>
    {
        Task<IEnumerable<VaccineHistory>> GetByUserIdAsync(string userId);
        Task<IEnumerable<VaccineHistory>> SearchByDateAsync(string userId, DateTime date);
        Task<IEnumerable<VaccineHistory>> SearchByCenterIdAsync(string centerId);
    }




}
