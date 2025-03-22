using AutoMapper;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Dashboard;
using ModelViews.Responses.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVaccineScheduleRepository _vaccinationScheduleRepository;
        private readonly IOrderVaccineDetailsRepository _orderVaccineDetailsRepository;
        private readonly IOrderPackageDetailsRepository _orderPackageDetailsRepository;
        private readonly IMapper _mapper;

        public DashboardService(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IChildrenProfileRepository childrenProfileRepository,
            IVaccineCenterRepository vaccineCenterRepository,
            IVaccineRepository vaccineRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IVaccineScheduleRepository vaccinationScheduleRepository,
            IOrderVaccineDetailsRepository orderVaccineDetailsRepository,
            IOrderPackageDetailsRepository orderPackageDetailsRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _childrenProfileRepository = childrenProfileRepository;
            _vaccineCenterRepository = vaccineCenterRepository;
            _vaccineRepository = vaccineRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _vaccinationScheduleRepository = vaccinationScheduleRepository;
            _orderVaccineDetailsRepository = orderVaccineDetailsRepository;
            _orderPackageDetailsRepository = orderPackageDetailsRepository;
            _mapper = mapper;
        }

        public async Task<DashboardResponseDTO> GetDashboardDataAsync()
        {
            // Existing implementation remains unchanged
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

        public async Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByDayAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.Entities
                .Where(o => o.PurchaseDate.Date >= startDate.Date && o.PurchaseDate.Date <= endDate.Date)
                .Include(o => o.OrderVaccineDetails)
                .Include(o => o.OrderPackageDetails)
                .ToListAsync();

            var payments = await _paymentRepository.Entities
                .Where(p => p.PaymentDate.Date >= startDate.Date && p.PaymentDate.Date <= endDate.Date && p.PaymentStatus == "Success")
                .ToListAsync();

            var result = from date in Enumerable.Range(0, (endDate.Date - startDate.Date).Days + 1)
                         .Select(d => startDate.Date.AddDays(d))
                         let dayOrders = orders.Where(o => o.PurchaseDate.Date == date)
                         let dayPayments = payments.Where(p => p.PaymentDate.Date == date)
                         select new RevenueAndOrderResponseDTO
                         {
                             Period = date.ToString("yyyy-MM-dd"),
                             TotalOrders = dayOrders.Count(),
                             TotalOrderAmount = dayOrders.Sum(o => o.TotalOrderPrice),
                             TotalRevenue = dayPayments.Sum(p => p.PayAmount),
                             VaccineQuantity = dayOrders.Sum(o =>
                                 (o.OrderVaccineDetails?.Sum(ov => ov.Quantity) ?? 0) +
                                 (o.OrderPackageDetails?.Sum(op => op.Quantity) ?? 0)),
                             PaymentName = dayPayments.GroupBy(p => p.PaymentName)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault() ?? "N/A"
                         };

            return result;
        }

        public async Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByMonthAsync(int year)
        {
            var orders = await _orderRepository.Entities
                .Where(o => o.PurchaseDate.Year == year)
                .Include(o => o.OrderVaccineDetails)
                .Include(o => o.OrderPackageDetails)
                .ToListAsync();

            var payments = await _paymentRepository.Entities
                .Where(p => p.PaymentDate.Year == year && p.PaymentStatus == "Success")
                .ToListAsync();

            var result = from month in Enumerable.Range(1, 12)
                         let monthDate = new DateTime(year, month, 1)
                         let monthOrders = orders.Where(o => o.PurchaseDate.Year == year && o.PurchaseDate.Month == month)
                         let monthPayments = payments.Where(p => p.PaymentDate.Year == year && p.PaymentDate.Month == month)
                         select new RevenueAndOrderResponseDTO
                         {
                             Period = monthDate.ToString("yyyy-MM"),
                             TotalOrders = monthOrders.Count(),
                             TotalOrderAmount = monthOrders.Sum(o => o.TotalOrderPrice),
                             TotalRevenue = monthPayments.Sum(p => p.PayAmount),
                             VaccineQuantity = monthOrders.Sum(o =>
                                 (o.OrderVaccineDetails?.Sum(ov => ov.Quantity) ?? 0) +
                                 (o.OrderPackageDetails?.Sum(op => op.Quantity) ?? 0)),
                             PaymentName = monthPayments.GroupBy(p => p.PaymentName)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault() ?? "N/A"
                         };

            return result;
        }

        public async Task<IEnumerable<RevenueAndOrderResponseDTO>> GetRevenueAndOrdersByYearAsync(int startYear, int endYear)
        {
            var orders = await _orderRepository.Entities
                .Where(o => o.PurchaseDate.Year >= startYear && o.PurchaseDate.Year <= endYear)
                .Include(o => o.OrderVaccineDetails)
                .Include(o => o.OrderPackageDetails)
                .ToListAsync();

            var payments = await _paymentRepository.Entities
                .Where(p => p.PaymentDate.Year >= startYear && p.PaymentDate.Year <= endYear && p.PaymentStatus == "Success")
                .ToListAsync();

            var result = from year in Enumerable.Range(startYear, endYear - startYear + 1)
                         let yearOrders = orders.Where(o => o.PurchaseDate.Year == year)
                         let yearPayments = payments.Where(p => p.PaymentDate.Year == year)
                         select new RevenueAndOrderResponseDTO
                         {
                             Period = year.ToString("yyyy"),
                             TotalOrders = yearOrders.Count(),
                             TotalOrderAmount = yearOrders.Sum(o => o.TotalOrderPrice),
                             TotalRevenue = yearPayments.Sum(p => p.PayAmount),
                             VaccineQuantity = yearOrders.Sum(o =>
                                 (o.OrderVaccineDetails?.Sum(ov => ov.Quantity) ?? 0) +
                                 (o.OrderPackageDetails?.Sum(op => op.Quantity) ?? 0)),
                             PaymentName = yearPayments.GroupBy(p => p.PaymentName)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault() ?? "N/A"
                         };

            return result;
        }
    }
}