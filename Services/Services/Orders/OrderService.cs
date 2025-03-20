using AutoMapper;
using IRepositories.Entity.Orders;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Orders;
using Microsoft.EntityFrameworkCore;
using ModelViews.Requests.Order;
using ModelViews.Requests.Payment;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderVaccineDetailsRepository _orderVaccineDetailsRepository;
        private readonly IOrderPackageDetailsRepository _orderPackageDetailsRepository;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IVaccinePackageRepository _vaccinePackageRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IOrderVaccineDetailsRepository orderVaccineDetailsRepository,
            IOrderPackageDetailsRepository orderPackageDetailsRepository,
            IVaccineRepository vaccineRepository,
            IVaccinePackageRepository vaccinePackageRepository,
            IChildrenProfileRepository childrenProfileRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderVaccineDetailsRepository = orderVaccineDetailsRepository ?? throw new ArgumentNullException(nameof(orderVaccineDetailsRepository));
            _orderPackageDetailsRepository = orderPackageDetailsRepository ?? throw new ArgumentNullException(nameof(orderPackageDetailsRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _vaccinePackageRepository = vaccinePackageRepository ?? throw new ArgumentNullException(nameof(vaccinePackageRepository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.Entities
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Order ID cannot be null or empty.");

            var order = await _orderRepository.Entities
                .Where(o => o.Id == id)
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .FirstOrDefaultAsync();

            return order == null ? null : _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByParentIdAsync(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                throw new ArgumentException("Parent ID cannot be null or empty.");

            var childrenProfiles = await _childrenProfileRepository.Entities
                .Where(cp => cp.AccountId == parentId)
                .Select(cp => cp.Id)
                .ToListAsync();

            if (!childrenProfiles.Any())
                return new List<OrderResponseDTO>();

            var orders = await _orderRepository.Entities
                .Where(o => childrenProfiles.Contains(o.ProfileId))
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetPaidOrdersByParentIdAsync(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                throw new ArgumentException("Parent ID cannot be null or empty.");

            var childrenProfiles = await _childrenProfileRepository.Entities
                .Where(cp => cp.AccountId == parentId)
                .Select(cp => cp.Id)
                .ToListAsync();

            if (!childrenProfiles.Any())
                return new List<OrderResponseDTO>();

            var orders = await _orderRepository.Entities
                .Where(o => childrenProfiles.Contains(o.ProfileId) && o.Status == "Paid")
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Order request cannot be null.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null)
                    throw new Exception("Profile does not exist.");

                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileId = request.ProfileId,
                    PurchaseDate = request.PurchaseDate,
                    TotalAmount = 0,
                    TotalOrderPrice = 0,
                    Status = "Pending"
                };

                await _orderRepository.InsertAsync(order);

                int totalAmount = 0;
                int totalOrderPrice = 0;

                foreach (var vaccineItem in request.Vaccines)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineItem.VaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with Id {vaccineItem.VaccineId} not found or inactive.");
                    if (vaccine.QuantityAvailable < vaccineItem.Quantity)
                        throw new Exception($"Insufficient quantity for vaccine {vaccine.Name}.");

                    var orderVaccineDetail = new OrderVaccineDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccineId = vaccineItem.VaccineId,
                        Quantity = vaccineItem.Quantity,
                        TotalPrice = vaccine.Price * vaccineItem.Quantity
                    };

                    await _orderVaccineDetailsRepository.InsertAsync(orderVaccineDetail);
                    vaccine.QuantityAvailable -= vaccineItem.Quantity;
                    await _vaccineRepository.UpdateAsync(vaccine);

                    totalAmount += vaccineItem.Quantity;
                    totalOrderPrice += orderVaccineDetail.TotalPrice;
                }

                foreach (var packageItem in request.VaccinePackages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null || !package.PackageStatus)
                        throw new Exception($"VaccinePackage with Id {packageItem.VaccinePackageId} not found or inactive.");

                    var orderPackageDetail = new OrderPackageDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccinePackageId = packageItem.VaccinePackageId,
                        Quantity = packageItem.Quantity,
                        TotalPrice = package.PackagePrice * packageItem.Quantity
                    };

                    await _orderPackageDetailsRepository.InsertAsync(orderPackageDetail);

                    totalAmount += packageItem.Quantity;
                    totalOrderPrice += orderPackageDetail.TotalPrice;
                }

                order.TotalAmount = totalAmount;
                order.TotalOrderPrice = totalOrderPrice;
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var createdOrder = await _orderRepository.Entities
                    .Where(o => o.Id == order.Id)
                    .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                    .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                    .FirstOrDefaultAsync();

                return _mapper.Map<OrderResponseDTO>(createdOrder);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create order: {ex.Message}", ex);
            }
        }

        public async Task<OrderResponseDTO> AddOrderDetailsAsync(AddOrderDetailsRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Order ID and details are required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order does not exist.");

                foreach (var vaccineItem in request.Vaccines)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineItem.VaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with Id {vaccineItem.VaccineId} not found or inactive.");
                    if (vaccine.QuantityAvailable < vaccineItem.Quantity)
                        throw new Exception($"Insufficient quantity for vaccine {vaccine.Name}.");

                    var orderVaccineDetail = new OrderVaccineDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccineId = vaccineItem.VaccineId,
                        Quantity = vaccineItem.Quantity,
                        TotalPrice = vaccine.Price * vaccineItem.Quantity
                    };

                    await _orderVaccineDetailsRepository.InsertAsync(orderVaccineDetail);
                    vaccine.QuantityAvailable -= vaccineItem.Quantity;
                    await _vaccineRepository.UpdateAsync(vaccine);
                }

                foreach (var packageItem in request.Packages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null || !package.PackageStatus)
                        throw new Exception($"VaccinePackage with Id {packageItem.VaccinePackageId} not found or inactive.");

                    var orderPackageDetail = new OrderPackageDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccinePackageId = packageItem.VaccinePackageId,
                        Quantity = packageItem.Quantity,
                        TotalPrice = package.PackagePrice * packageItem.Quantity
                    };

                    await _orderPackageDetailsRepository.InsertAsync(orderPackageDetail);
                }

                var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                    .Where(vd => vd.OrderId == order.Id)
                    .ToListAsync();
                var packageDetails = await _orderPackageDetailsRepository.Entities
                    .Where(pd => pd.OrderId == order.Id)
                    .ToListAsync();

                order.TotalAmount = vaccineDetails.Sum(vd => vd.Quantity) + packageDetails.Sum(pd => pd.Quantity);
                order.TotalOrderPrice = vaccineDetails.Sum(vd => vd.TotalPrice) + packageDetails.Sum(pd => pd.TotalPrice);
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var updatedOrder = await _orderRepository.Entities
                    .Where(o => o.Id == order.Id)
                    .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                    .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                    .FirstOrDefaultAsync();

                return _mapper.Map<OrderResponseDTO>(updatedOrder);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to add order details: {ex.Message}", ex);
            }
        }

        public async Task<OrderResponseDTO> RemoveOrderDetailsAsync(RemoveOrderDetailsRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Order ID is required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order does not exist.");

                foreach (var detailId in request.VaccineDetailIds ?? new List<string>())
                {
                    var detail = await _orderVaccineDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id)
                    {
                        var vaccine = await _vaccineRepository.GetByIdAsync(detail.VaccineId);
                        if (vaccine != null)
                        {
                            vaccine.QuantityAvailable += detail.Quantity;
                            await _vaccineRepository.UpdateAsync(vaccine);
                        }
                        await _orderVaccineDetailsRepository.DeleteAsync(detail.Id);
                    }
                }

                foreach (var detailId in request.PackageDetailIds ?? new List<string>())
                {
                    var detail = await _orderPackageDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id)
                    {
                        await _orderPackageDetailsRepository.DeleteAsync(detail.Id);
                    }
                }

                var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                    .Where(vd => vd.OrderId == order.Id)
                    .ToListAsync();
                var packageDetails = await _orderPackageDetailsRepository.Entities
                    .Where(pd => pd.OrderId == order.Id)
                    .ToListAsync();

                order.TotalAmount = vaccineDetails.Sum(vd => vd.Quantity) + packageDetails.Sum(pd => pd.Quantity);
                order.TotalOrderPrice = vaccineDetails.Sum(vd => vd.TotalPrice) + packageDetails.Sum(pd => pd.TotalPrice);
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var updatedOrder = await _orderRepository.Entities
                    .Where(o => o.Id == order.Id)
                    .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                    .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                    .FirstOrDefaultAsync();

                return _mapper.Map<OrderResponseDTO>(updatedOrder);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to remove order details: {ex.Message}", ex);
            }
        }

        public async Task<OrderResponseDTO> SetPayLaterAsync(PayLaterRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Order ID is required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order does not exist.");
                if (order.Status != "Pending")
                    throw new Exception("Can only set PayLater from Pending status.");

                order.Status = "PayLater";
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var updatedOrder = await _orderRepository.Entities
                    .Where(o => o.Id == order.Id)
                    .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                    .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                    .FirstOrDefaultAsync();

                return _mapper.Map<OrderResponseDTO>(updatedOrder);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to set pay later: {ex.Message}", ex);
            }
        }
    }
}