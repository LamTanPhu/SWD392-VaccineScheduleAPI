using AutoMapper;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Dashboard;
using ModelViews.Responses.Dashboard;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IRepositories.IRepository.Inventory;
using IRepositories.IRepository.Schedules;
using IRepositories.Enum;

namespace Services.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IVaccineCenterRepository _vaccineCenterRepository;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVaccineScheduleRepository _vaccinationScheduleRepository;
        private readonly IMapper _mapper;

        public DashboardService(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IChildrenProfileRepository childrenProfileRepository,
            IVaccineCenterRepository vaccineCenterRepository,
            IVaccineRepository vaccineRepository,
            IOrderRepository orderRepository,
            IVaccineScheduleRepository vaccinationScheduleRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _childrenProfileRepository = childrenProfileRepository;
            _vaccineCenterRepository = vaccineCenterRepository;
            _vaccineRepository = vaccineRepository;
            _orderRepository = orderRepository;
            _vaccinationScheduleRepository = vaccinationScheduleRepository;
            _mapper = mapper;
        }

        public async Task<DashboardResponseDTO> GetDashboardDataAsync()
        {
            var accounts = await _accountRepository.Entities.ToListAsync();
            var totalAccounts = accounts.Count;
            var adminAccounts = accounts.Count(a => a.Role == RoleEnum.Admin);
            var staffAccounts = accounts.Count(a => a.Role == RoleEnum.Staff);
            var parentAccounts = accounts.Count(a => a.Role == RoleEnum.Parent);

            var totalChildrenProfile = await _childrenProfileRepository.Entities.CountAsync();
            var totalVaccineCenters = await _vaccineCenterRepository.Entities.CountAsync();
            var totalVaccinesAvailable = await _vaccineRepository.Entities.SumAsync(v => v.QuantityAvailable);
            var totalOrdersPaid = await _orderRepository.Entities.CountAsync(o => o.Status == "Paid");
            var totalSchedule = await _vaccinationScheduleRepository.Entities.CountAsync();

            return new DashboardResponseDTO
            {
                TotalAccounts = totalAccounts,
                AdminAccounts = adminAccounts,
                StaffAccounts = staffAccounts,
                ParentAccounts = parentAccounts,
                TotalChildrenProfile = totalChildrenProfile,
                TotalVaccineCenters = totalVaccineCenters,
                TotalVaccinesAvailable = totalVaccinesAvailable,
                TotalOrdersPaid = totalOrdersPaid,
                TotalSchedule = totalSchedule
            };
        }
    }
}