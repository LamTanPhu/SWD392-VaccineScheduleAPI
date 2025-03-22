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
        Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByDayAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByMonthAsync(int year);
        Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByYearAsync(int startYear, int endYear);
    }
}
