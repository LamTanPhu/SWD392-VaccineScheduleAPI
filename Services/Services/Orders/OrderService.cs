using IRepositories.Entity.Orders;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository;
using IServices.Interfaces.Orders;
using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Entity.Vaccines;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using Microsoft.EntityFrameworkCore;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Inventory;
using IRepositories.IRepository.Vaccines;
using ModelViews.Requests;

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
        private readonly IVaccinePackageDetailsRepository _vaccinePackageDetailRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IVaccineCenterRepository _vaccineCenterRepository;

        public OrderService(IUnitOfWork unitOfWork,
                           IOrderRepository orderRepository,
                           IOrderVaccineDetailsRepository orderVaccineDetailsRepository,
                           IOrderPackageDetailsRepository orderPackageDetailsRepository,
                           IVaccineRepository vaccineRepository,
                           IVaccinePackageRepository vaccinePackageRepository,
                           IVaccinePackageDetailsRepository vaccinePackageDetailRepository,
                           IChildrenProfileRepository childrenProfileRepository,
                           IVaccineCenterRepository vaccineCenterRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderVaccineDetailsRepository = orderVaccineDetailsRepository ?? throw new ArgumentNullException(nameof(orderVaccineDetailsRepository));
            _orderPackageDetailsRepository = orderPackageDetailsRepository ?? throw new ArgumentNullException(nameof(orderPackageDetailsRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _vaccinePackageRepository = vaccinePackageRepository ?? throw new ArgumentNullException(nameof(vaccinePackageRepository));
            _vaccinePackageDetailRepository = vaccinePackageDetailRepository ?? throw new ArgumentNullException(nameof(vaccinePackageDetailRepository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _vaccineCenterRepository = vaccineCenterRepository ?? throw new ArgumentNullException(nameof(vaccineCenterRepository));
        }


        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync()
        {
            // Lấy tất cả orders từ repository
            var orders = await _orderRepository.GetAllAsync();

            // Lấy tất cả OrderVaccineDetails và OrderPackageDetails một lần để tối ưu
            var allVaccineDetails = await _orderVaccineDetailsRepository.Entities
                .Where(vd => vd.DeletedTime == null)
                .ToListAsync();

            var allPackageDetails = await _orderPackageDetailsRepository.Entities
                .Where(pd => pd.DeletedTime == null)
                .ToListAsync();

            // Ánh xạ sang OrderResponseDTO
            return orders.Select(o =>
            {
                var vaccineDetails = allVaccineDetails
                    .Where(vd => vd.OrderId == o.Id)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        VaccineId = vd.VaccineId,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice
                    })
                    .ToList();

                var packageDetails = allPackageDetails
                    .Where(pd => pd.OrderId == o.Id)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        VaccinePackageId = pd.VaccinePackageId,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    })
                    .ToList();

                return new OrderResponseDTO
                {
                    OrderId = o.Id,
                    ProfileId = o.ProfileId,
                    PurchaseDate = o.PurchaseDate,
                    TotalAmount = o.TotalAmount,
                    TotalOrderPrice = o.TotalOrderPrice,
                    Status = o.Status,
                    VaccineDetails = vaccineDetails,
                    PackageDetails = packageDetails
                };
            }).ToList();
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;
            // Fetch chi tiết OrderVaccineDetails và OrderPackageDetails
            var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                .Where(vd => vd.OrderId == order.Id && vd.DeletedTime == null)
                .Select(vd => new OrderVaccineDetailResponseDTO
                {
                    VaccineId = vd.VaccineId,
                    Quantity = vd.Quantity,
                    TotalPrice = vd.TotalPrice
                })
                .ToListAsync();

            var packageDetails = await _orderPackageDetailsRepository.Entities
                .Where(pd => pd.OrderId == order.Id && pd.DeletedTime == null)
                .Select(pd => new OrderPackageDetailResponseDTO
                {
                    VaccinePackageId = pd.VaccinePackageId,
                    Quantity = pd.Quantity,
                    TotalPrice = pd.TotalPrice
                })
                .ToListAsync();
            return new OrderResponseDTO
            {
                OrderId = order.Id,
                ProfileId = order.ProfileId,
                PurchaseDate = order.PurchaseDate,
                TotalAmount = order.TotalAmount,
                TotalOrderPrice = order.TotalOrderPrice,
                Status = order.Status,
                VaccineDetails = vaccineDetails,
                PackageDetails = packageDetails
            };
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate ProfileId và CenterId
                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null)
                    throw new Exception("Profile không tồn tại.");

                // Tạo Order entity từ DTO
                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileId = request.ProfileId,
                    PurchaseDate = request.PurchaseDate,
                    TotalAmount = 0,
                    TotalOrderPrice = 0,
                    Status = "Pending",
                };

                await _orderRepository.InsertAsync(order);
                await _unitOfWork.SaveAsync();

                int totalAmount = 0;
                int totalOrderPrice = 0;

                // Xử lý vaccine lẻ
                foreach (var vaccineItem in request.Vaccines)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineItem.VaccineId);
                    if (vaccine == null)
                        throw new Exception($"Không tìm thấy Vaccine với Id: {vaccineItem.VaccineId}");
                    if (vaccine.QuantityAvailable < vaccineItem.Quantity)
                        throw new Exception($"Số lượng Vaccine {vaccine.Name} không đủ.");

                    var orderVaccineDetail = new OrderVaccineDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccineId = vaccineItem.VaccineId,
                        Quantity = vaccineItem.Quantity,
                        TotalPrice = vaccine.Price * vaccineItem.Quantity,
                    };

                    await _orderVaccineDetailsRepository.InsertAsync(orderVaccineDetail);

                    vaccine.QuantityAvailable -= vaccineItem.Quantity;
                    await _vaccineRepository.UpdateAsync(vaccine);

                    totalAmount += vaccineItem.Quantity;
                    totalOrderPrice += orderVaccineDetail.TotalPrice;
                }

                // Xử lý vaccine gói
                foreach (var packageItem in request.VaccinePackages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null)
                        throw new Exception($"Không tìm thấy VaccinePackage với Id: {packageItem.VaccinePackageId}");

                    var packagePrice = await _vaccinePackageDetailRepository.Entities
                        .Where(d => d.VaccinePackageId == packageItem.VaccinePackageId)
                        .SumAsync(d => d.PackagePrice);

                    var orderPackageDetail = new OrderPackageDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccinePackageId = packageItem.VaccinePackageId,
                        Quantity = packageItem.Quantity,
                        TotalPrice = packagePrice * packageItem.Quantity,
                    };

                    await _orderPackageDetailsRepository.InsertAsync(orderPackageDetail);

                    totalAmount += packageItem.Quantity;
                    totalOrderPrice += orderPackageDetail.TotalPrice;
                }

                // Cập nhật TotalAmount và TotalOrderPrice
                order.TotalAmount = totalAmount;
                order.TotalOrderPrice = totalOrderPrice;


                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Fetch chi tiết OrderVaccineDetails và OrderPackageDetails
                var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                    .Where(vd => vd.OrderId == order.Id && vd.DeletedTime == null)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        VaccineId = vd.VaccineId,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice
                    })
                    .ToListAsync();

                var packageDetails = await _orderPackageDetailsRepository.Entities
                    .Where(pd => pd.OrderId == order.Id && pd.DeletedTime == null)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        VaccinePackageId = pd.VaccinePackageId,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    })
                    .ToListAsync();

                // Ánh xạ sang ResponseDTO
                return new OrderResponseDTO
                {
                    OrderId = order.Id,
                    ProfileId = order.ProfileId,
                    PurchaseDate = order.PurchaseDate,
                    TotalAmount = order.TotalAmount,
                    TotalOrderPrice = order.TotalOrderPrice,
                    Status = order.Status,
                    VaccineDetails = vaccineDetails,
                    PackageDetails = packageDetails
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderResponseDTO> AddOrderDetailsAsync(AddOrderDetailsRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");

                // Thêm OrderVaccineDetails
                foreach (var vaccineItem in request.Vaccines)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineItem.VaccineId);
                    if (vaccine == null)
                        throw new Exception($"Không tìm thấy Vaccine với Id: {vaccineItem.VaccineId}");
                    if (vaccine.QuantityAvailable < vaccineItem.Quantity)
                        throw new Exception($"Số lượng Vaccine {vaccine.Name} không đủ.");

                    var orderVaccineDetail = new OrderVaccineDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccineId = vaccineItem.VaccineId,
                        Quantity = vaccineItem.Quantity,
                        TotalPrice = vaccine.Price * vaccineItem.Quantity,
                    };

                    await _orderVaccineDetailsRepository.InsertAsync(orderVaccineDetail);
                    vaccine.QuantityAvailable -= vaccineItem.Quantity;
                    await _vaccineRepository.UpdateAsync(vaccine);
                }

                // Thêm OrderPackageDetails
                foreach (var packageItem in request.Packages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null)
                        throw new Exception($"Không tìm thấy VaccinePackage với Id: {packageItem.VaccinePackageId}");

                    var packagePrice = await _vaccinePackageDetailRepository.Entities
                        .Where(d => d.VaccinePackageId == packageItem.VaccinePackageId)
                        .SumAsync(d => d.PackagePrice);

                    var orderPackageDetail = new OrderPackageDetails
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        VaccinePackageId = packageItem.VaccinePackageId,
                        Quantity = packageItem.Quantity,
                        TotalPrice = packagePrice * packageItem.Quantity,
                    };

                    await _orderPackageDetailsRepository.InsertAsync(orderPackageDetail);
                }

                // Cập nhật TotalAmount và TotalOrderPrice
                var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                    .Where(vd => vd.OrderId == order.Id && vd.DeletedTime == null)
                    .ToListAsync();
                var packageDetails = await _orderPackageDetailsRepository.Entities
                    .Where(pd => pd.OrderId == order.Id && pd.DeletedTime == null)
                    .ToListAsync();

                order.TotalAmount = vaccineDetails.Sum(vd => vd.Quantity) + packageDetails.Sum(pd => pd.Quantity);
                order.TotalOrderPrice = vaccineDetails.Sum(vd => vd.TotalPrice) + packageDetails.Sum(pd => pd.TotalPrice);
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Thêm Order Details thất bại: {ex.Message}");
            }
        }

        public async Task<OrderResponseDTO> RemoveOrderDetailsAsync(RemoveOrderDetailsRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");

                // Xóa OrderVaccineDetails
                foreach (var detailId in request.VaccineDetailIds)
                {
                    var detail = await _orderVaccineDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id && detail.DeletedTime == null)
                    {
                        var vaccine = await _vaccineRepository.GetByIdAsync(detail.VaccineId);
                        if (vaccine != null)
                        {
                            vaccine.QuantityAvailable += detail.Quantity; // Hoàn lại kho
                            await _vaccineRepository.UpdateAsync(vaccine);
                        }
                        detail.DeletedTime = DateTime.Now; // Soft delete
                        await _orderVaccineDetailsRepository.UpdateAsync(detail);
                    }
                }

                // Xóa OrderPackageDetails
                foreach (var detailId in request.PackageDetailIds)
                {
                    var detail = await _orderPackageDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id && detail.DeletedTime == null)
                    {
                        detail.DeletedTime = DateTime.Now; // Soft delete
                        await _orderPackageDetailsRepository.UpdateAsync(detail);
                    }
                }

                // Cập nhật TotalAmount và TotalOrderPrice
                var vaccineDetails = await _orderVaccineDetailsRepository.Entities
                    .Where(vd => vd.OrderId == order.Id && vd.DeletedTime == null)
                    .ToListAsync();
                var packageDetails = await _orderPackageDetailsRepository.Entities
                    .Where(pd => pd.OrderId == order.Id && pd.DeletedTime == null)
                    .ToListAsync();

                order.TotalAmount = vaccineDetails.Sum(vd => vd.Quantity) + packageDetails.Sum(pd => pd.Quantity);
                order.TotalOrderPrice = vaccineDetails.Sum(vd => vd.TotalPrice) + packageDetails.Sum(pd => pd.TotalPrice);
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Xóa Order Details thất bại: {ex.Message}");
            }
        }

        public async Task<OrderResponseDTO> SetPayLaterAsync(PayLaterRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");


                if (order.Status != "Pending")
                    throw new Exception("Chỉ có thể chuyển sang PayLater từ trạng thái Pending.");


                order.Status = "PayLater";
                order.LastUpdatedTime = DateTime.Now; 

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();


                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Chuyển trạng thái sang PayLater thất bại: {ex.Message}");
            }
        }



    }
}
