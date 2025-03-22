using ModelViews.Responses.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardResponseDTO> GetDashboardDataAsync();
    }
}
