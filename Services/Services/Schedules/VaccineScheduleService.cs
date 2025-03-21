namespace Services.Services.Schedules
{
    using IRepositories.IRepository;
    using IRepositories.IRepository.Schedules;
    using IRepositories.IRepository.Orders;
    using IRepositories.IRepository.Accounts;
    using IRepositories.Entity.Schedules;
    using IServices.Interfaces.Schedules;
    using ModelViews.Requests.Schedule;
    using ModelViews.Responses.Schedule;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class VaccinationScheduleService : IVaccinationScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineScheduleRepository _scheduleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderVaccineDetailsRepository _orderVaccineDetailsRepository;
        private readonly IOrderPackageDetailsRepository _orderPackageDetailsRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;

        public VaccinationScheduleService(
            IUnitOfWork unitOfWork,
            IVaccineScheduleRepository scheduleRepository,
            IOrderRepository orderRepository,
            IOrderVaccineDetailsRepository orderVaccineDetailsRepository,
            IOrderPackageDetailsRepository orderPackageDetailsRepository,
            IChildrenProfileRepository childrenProfileRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderVaccineDetailsRepository = orderVaccineDetailsRepository ?? throw new ArgumentNullException(nameof(orderVaccineDetailsRepository));
            _orderPackageDetailsRepository = orderPackageDetailsRepository ?? throw new ArgumentNullException(nameof(orderPackageDetailsRepository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
        }

        
        public async Task<List<ScheduleResponseDTO>> CreateSchedulesAsync(ScheduleRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId) || request.Schedules == null || !request.Schedules.Any())
                throw new ArgumentException("Invalid request data.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order not found.");

                var profile = await _childrenProfileRepository.GetByIdAsync(order.ProfileId);
                if (profile == null)
                    throw new Exception("Profile not found.");

                var createdSchedules = new List<ScheduleResponseDTO>();
                var currentDate = DateTime.UtcNow;

                foreach (var scheduleItem in request.Schedules)
                {
                    if (scheduleItem.DoseNumber <= 0)
                        throw new Exception($"Invalid DoseNumber: {scheduleItem.DoseNumber}");
                    if (scheduleItem.AppointmentDate <= currentDate)
                        throw new Exception($"AppointmentDate must be in the future: {scheduleItem.AppointmentDate}");
                    if (string.IsNullOrEmpty(scheduleItem.VaccineCenterId))
                        throw new Exception("VaccineCenterId is required.");

                    string? vaccineId = null;

                    if (!string.IsNullOrEmpty(scheduleItem.OrderVaccineDetailsId))
                    {
                        var vaccineDetail = await _orderVaccineDetailsRepository.GetByIdAsync(scheduleItem.OrderVaccineDetailsId);
                        if (vaccineDetail == null || vaccineDetail.OrderId != request.OrderId)
                            throw new Exception($"Invalid OrderVaccineDetailsId: {scheduleItem.OrderVaccineDetailsId}");
                        vaccineId = vaccineDetail.VaccineId;
                    }
                    else if (!string.IsNullOrEmpty(scheduleItem.OrderPackageDetailsId))
                    {
                        var packageDetail = await _orderPackageDetailsRepository.GetByIdWithPackageDetailsAsync(scheduleItem.OrderPackageDetailsId);
                        if (packageDetail == null || packageDetail.OrderId != request.OrderId)
                            throw new Exception($"Invalid OrderPackageDetailsId: {scheduleItem.OrderPackageDetailsId}");

                    }
                    else
                    {
                        throw new Exception("Must specify either OrderVaccineDetailsId or OrderPackageDetailsId.");
                    }

                    var exists = await _scheduleRepository.ExistsAsync(
                        order.ProfileId,
                        scheduleItem.OrderVaccineDetailsId,
                        scheduleItem.OrderPackageDetailsId,
                        scheduleItem.DoseNumber);
                    if (exists)
                        throw new Exception($"Schedule already exists for Dose {scheduleItem.DoseNumber}.");//this is where the problem is

                    var schedule = new VaccinationSchedule
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProfileId = order.ProfileId,
                        VaccineCenterId = scheduleItem.VaccineCenterId,
                        OrderVaccineDetailsId = scheduleItem.OrderVaccineDetailsId,
                        OrderPackageDetailsId = scheduleItem.OrderPackageDetailsId,
                        DoseNumber = scheduleItem.DoseNumber,
                        AppointmentDate = scheduleItem.AppointmentDate,
                        ActualDate = null,
                        AdministeredBy = null,
                        Status = 1 
                    };

                    await _scheduleRepository.InsertAsync(schedule);
                    createdSchedules.Add(MapToResponseDTO(schedule));
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return createdSchedules;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create schedules: {ex.Message}");
            }
        }

        public async Task<ScheduleResponseDTO> CreateScheduleAsync(CreateScheduleRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.ProfileId) || string.IsNullOrEmpty(request.VaccineCenterId))
                throw new ArgumentException("Invalid request data: ProfileId and VaccineCenterId are required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                
                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null)
                    throw new Exception($"Profile with ID {request.ProfileId} not found.");


                string? vaccineId = request.VaccineId;
                if (!string.IsNullOrEmpty(request.OrderVaccineDetailsId))
                {
                    var vaccineDetail = await _orderVaccineDetailsRepository.GetByIdAsync(request.OrderVaccineDetailsId);
                    if (vaccineDetail == null)
                        throw new Exception($"Invalid OrderVaccineDetailsId: {request.OrderVaccineDetailsId}");
                    vaccineId = vaccineDetail.VaccineId; 
                }
                else if (!string.IsNullOrEmpty(request.OrderPackageDetailsId))
                {
                    var packageDetail = await _orderPackageDetailsRepository.GetByIdWithPackageDetailsAsync(request.OrderPackageDetailsId);
                    if (packageDetail == null)
                        throw new Exception($"Invalid OrderPackageDetailsId: {request.OrderPackageDetailsId}");

                }

                
                var currentDate = DateTime.UtcNow;
                if (request.DoseNumber <= 0)
                    throw new Exception($"Invalid DoseNumber: {request.DoseNumber}");
                if (request.AppointmentDate <= currentDate && request.Status == 1)
                    throw new Exception("AppointmentDate must be in the future for status 'đặt lịch nhưng chưa đến'.");
                if (request.Status < 0 || request.Status > 2)
                    throw new Exception($"Invalid Status: {request.Status}. Must be 0, 1, or 2.");

                
                var exists = await _scheduleRepository.ExistsAsync(
                    request.ProfileId,
                    request.OrderVaccineDetailsId,
                    request.OrderPackageDetailsId,
                    request.DoseNumber);
                if (exists)
                    throw new Exception($"Schedule already exists for Dose {request.DoseNumber}.");

                
                var schedule = new VaccinationSchedule
                {
                    Id = string.IsNullOrEmpty(request.Id) ? Guid.NewGuid().ToString() : request.Id,
                    ProfileId = request.ProfileId,
                    VaccineCenterId = request.VaccineCenterId,
                    OrderVaccineDetailsId = request.OrderVaccineDetailsId,
                    OrderPackageDetailsId = request.OrderPackageDetailsId,
                    DoseNumber = request.DoseNumber,
                    AppointmentDate = request.AppointmentDate,
                    ActualDate = request.ActualDate,
                    AdministeredBy = request.AdministeredBy,
                    Status = request.Status
                };

                await _scheduleRepository.InsertAsync(schedule);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return MapToResponseDTO(schedule);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create schedule: {ex.Message}");
            }
        }

        
        public async Task<List<ScheduleResponseDTO>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return schedules.Select(MapToResponseDTO).ToList();
        }

        
        public async Task<ScheduleResponseDTO> GetScheduleByIdAsync(string id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception($"Schedule with ID {id} not found.");
            return MapToResponseDTO(schedule);
        }

        
        public async Task UpdateScheduleAsync(string scheduleId, UpdateScheduleRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(scheduleId))
                throw new ArgumentException("Invalid request data.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                    throw new Exception($"Schedule with ID {scheduleId} not found.");

                
                if (request.DoseNumber <= 0)
                    throw new Exception($"Invalid DoseNumber: {request.DoseNumber}");
                if (request.AppointmentDate <= DateTime.UtcNow && request.Status == 1)
                    throw new Exception("AppointmentDate must be in the future for status 'đặt lịch nhưng chưa đến'.");
                if (request.Status < 0 || request.Status > 2)
                    throw new Exception($"Invalid Status: {request.Status}. Must be 0, 1, or 2.");

                
                schedule.DoseNumber = request.DoseNumber;
                schedule.AppointmentDate = request.AppointmentDate;
                schedule.ActualDate = request.ActualDate;
                schedule.AdministeredBy = request.AdministeredBy;
                schedule.Status = request.Status;

                await _scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to update schedule: {ex.Message}");
            }
        }

       
        public async Task DeleteScheduleAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Schedule ID is required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                    throw new Exception($"Schedule with ID {id} not found.");

                schedule.Status = 0;
                await _scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to delete schedule: {ex.Message}");
            }
        }

        
        private ScheduleResponseDTO MapToResponseDTO(VaccinationSchedule schedule)
        {
            return new ScheduleResponseDTO
            {
                Id = schedule.Id,
                ProfileId = schedule.ProfileId,
                VaccineCenterId = schedule.VaccineCenterId,
                OrderVaccineDetailsId = schedule.OrderVaccineDetailsId,
                OrderPackageDetailsId = schedule.OrderPackageDetailsId,
                DoseNumber = schedule.DoseNumber,
                AppointmentDate = schedule.AppointmentDate,
                ActualDate = schedule.ActualDate,
                AdministeredBy = schedule.AdministeredBy,
                Status = schedule.Status
            };
        }
    }
}