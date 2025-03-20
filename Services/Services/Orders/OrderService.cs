using IRepositories.Entity.Orders;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Inventory;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository.Vaccines;
using IRepositories.IRepository;
using IServices.Interfaces.Orders;
using ModelViews.Requests.Order;
using ModelViews.Requests;
using ModelViews.Responses.Order;
using Microsoft.EntityFrameworkCore;

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

        // Các phương thức không thay đổi: GetAllOrdersAsync, GetOrderByIdAsync, GetOrdersByParentIdAsync
        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.Entities
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .Where(o => o.DeletedTime == null)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.Id,
                ProfileId = o.ProfileId,
                PurchaseDate = o.PurchaseDate,
                TotalAmount = o.TotalAmount,
                TotalOrderPrice = o.TotalOrderPrice,
                Status = o.Status,
                VaccineDetails = o.OrderVaccineDetails
                    .Where(vd => vd.DeletedTime == null)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        OrderVaccineId = vd.Id,
                        VaccineId = vd.VaccineId,
                        VaccineName = vd.Vaccine?.Name,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice,
                        Image = vd.Vaccine?.Image
                    }).ToList(),
                PackageDetails = o.OrderPackageDetails
                    .Where(pd => pd.DeletedTime == null)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        OrderPackageId = pd.Id,
                        VaccinePackageId = pd.VaccinePackageId,
                        VaccinePackageName = pd.VaccinePackage?.PackageName,
                        Description = pd.VaccinePackage?.PackageDescription,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    }).ToList()
            }).ToList();
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepository.Entities
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .FirstOrDefaultAsync(o => o.Id == id && o.DeletedTime == null);

            if (order == null) return null;

            return new OrderResponseDTO
            {
                OrderId = order.Id,
                ProfileId = order.ProfileId,
                PurchaseDate = order.PurchaseDate,
                TotalAmount = order.TotalAmount,
                TotalOrderPrice = order.TotalOrderPrice,
                Status = order.Status,
                VaccineDetails = order.OrderVaccineDetails
                    .Where(vd => vd.DeletedTime == null)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        OrderVaccineId = vd.Id,
                        VaccineId = vd.VaccineId,
                        VaccineName = vd.Vaccine?.Name,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice,
                        Image = vd.Vaccine?.Image
                    }).ToList(),
                PackageDetails = order.OrderPackageDetails
                    .Where(pd => pd.DeletedTime == null)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        OrderPackageId = pd.Id,
                        VaccinePackageId = pd.VaccinePackageId,
                        VaccinePackageName = pd.VaccinePackage?.PackageName,
                        Description = pd.VaccinePackage?.PackageDescription,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    }).ToList()
            };
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByParentIdAsync(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                throw new ArgumentException("Parent ID cannot be null or empty.");

            var childrenProfiles = await _childrenProfileRepository.Entities
                .Where(cp => cp.AccountId == parentId && cp.DeletedTime == null)
                .ToListAsync();

            if (!childrenProfiles.Any())
                return new List<OrderResponseDTO>();

            var profileIds = childrenProfiles.Select(cp => cp.Id).ToList();
            var orders = await _orderRepository.Entities
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .Where(o => profileIds.Contains(o.ProfileId) && o.DeletedTime == null)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.Id,
                ProfileId = o.ProfileId,
                PurchaseDate = o.PurchaseDate,
                TotalAmount = o.TotalAmount,
                TotalOrderPrice = o.TotalOrderPrice,
                Status = o.Status,
                VaccineDetails = o.OrderVaccineDetails
                    .Where(vd => vd.DeletedTime == null)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        OrderVaccineId = vd.Id,
                        VaccineId = vd.VaccineId,
                        VaccineName = vd.Vaccine?.Name,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice,
                        Image = vd.Vaccine?.Image
                    }).ToList(),
                PackageDetails = o.OrderPackageDetails
                    .Where(pd => pd.DeletedTime == null)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        OrderPackageId = pd.Id,
                        VaccinePackageId = pd.VaccinePackageId,
                        VaccinePackageName = pd.VaccinePackage?.PackageName,
                        Description = pd.VaccinePackage?.PackageDescription,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    }).ToList()
            }).ToList();
        }

        // Phương thức đã sửa: CreateOrderAsync
        public async Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null)
                    throw new Exception("Profile không tồn tại.");

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

                foreach (var packageItem in request.VaccinePackages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null)
                        throw new Exception($"Không tìm thấy VaccinePackage với Id: {packageItem.VaccinePackageId}");

                    // Sửa: Lấy PackagePrice trực tiếp từ VaccinePackage thay vì VaccinePackageDetail
                    var packagePrice = package.PackagePrice;

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

                order.TotalAmount = totalAmount;
                order.TotalOrderPrice = totalOrderPrice;
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
        }

        // Phương thức đã sửa: AddOrderDetailsAsync
        public async Task<OrderResponseDTO> AddOrderDetailsAsync(AddOrderDetailsRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");

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

                foreach (var packageItem in request.Packages)
                {
                    var package = await _vaccinePackageRepository.GetByIdAsync(packageItem.VaccinePackageId);
                    if (package == null)
                        throw new Exception($"Không tìm thấy VaccinePackage với Id: {packageItem.VaccinePackageId}");

                    // Sửa: Lấy PackagePrice trực tiếp từ VaccinePackage thay vì VaccinePackageDetail
                    var packagePrice = package.PackagePrice;

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

        // Các phương thức không thay đổi: RemoveOrderDetailsAsync, SetPayLaterAsync
        public async Task<OrderResponseDTO> RemoveOrderDetailsAsync(RemoveOrderDetailsRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");

                foreach (var detailId in request.VaccineDetailIds)
                {
                    var detail = await _orderVaccineDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id && detail.DeletedTime == null)
                    {
                        var vaccine = await _vaccineRepository.GetByIdAsync(detail.VaccineId);
                        if (vaccine != null)
                        {
                            vaccine.QuantityAvailable += detail.Quantity;
                            await _vaccineRepository.UpdateAsync(vaccine);
                        }
                        detail.DeletedTime = DateTime.Now;
                        await _orderVaccineDetailsRepository.UpdateAsync(detail);
                    }
                }

                foreach (var detailId in request.PackageDetailIds)
                {
                    var detail = await _orderPackageDetailsRepository.GetByIdAsync(detailId);
                    if (detail != null && detail.OrderId == order.Id && detail.DeletedTime == null)
                    {
                        detail.DeletedTime = DateTime.Now;
                        await _orderPackageDetailsRepository.UpdateAsync(detail);
                    }
                }

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

        public async Task<IEnumerable<OrderResponseDTO>> GetPaidOrdersByParentIdAsync(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                throw new ArgumentException("Parent ID cannot be null or empty.");

            // Fetch all ChildrenProfiles for the Parent
            var childrenProfiles = await _childrenProfileRepository.Entities
                .Where(cp => cp.AccountId == parentId && cp.DeletedTime == null)
                .ToListAsync();

            if (!childrenProfiles.Any())
                return new List<OrderResponseDTO>(); // Return empty list if no profiles exist

            // Fetch all "Paid" Orders for the ChildrenProfiles
            var profileIds = childrenProfiles.Select(cp => cp.Id).ToList();
            var orders = await _orderRepository.Entities
                .Include(o => o.OrderVaccineDetails).ThenInclude(vd => vd.Vaccine)
                .Include(o => o.OrderPackageDetails).ThenInclude(pd => pd.VaccinePackage)
                .Where(o => profileIds.Contains(o.ProfileId) && o.Status == "Paid" && o.DeletedTime == null)
                .ToListAsync();

            // Map to OrderResponseDTO
            return orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.Id,
                ProfileId = o.ProfileId,
                PurchaseDate = o.PurchaseDate,
                TotalAmount = o.TotalAmount,
                TotalOrderPrice = o.TotalOrderPrice,
                Status = o.Status,
                VaccineDetails = o.OrderVaccineDetails
                    .Where(vd => vd.DeletedTime == null)
                    .Select(vd => new OrderVaccineDetailResponseDTO
                    {
                        OrderVaccineId = vd.Id,
                        VaccineId = vd.VaccineId,
                        VaccineName = vd.Vaccine?.Name,
                        Quantity = vd.Quantity,
                        TotalPrice = vd.TotalPrice,
                        Image = vd.Vaccine?.Image
                    }).ToList(),
                PackageDetails = o.OrderPackageDetails
                    .Where(pd => pd.DeletedTime == null)
                    .Select(pd => new OrderPackageDetailResponseDTO
                    {
                        OrderPackageId = pd.Id,
                        VaccinePackageId = pd.VaccinePackageId,
                        VaccinePackageName = pd.VaccinePackage?.PackageName,
                        Description = pd.VaccinePackage?.PackageDescription,
                        Quantity = pd.Quantity,
                        TotalPrice = pd.TotalPrice
                    }).ToList()
            }).ToList();
        }
    }
}