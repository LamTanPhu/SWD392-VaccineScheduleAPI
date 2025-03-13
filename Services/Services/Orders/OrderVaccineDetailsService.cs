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

namespace Services.Services.Orders
{
    public class OrderVaccineDetailsService : IOrderVaccineDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderVaccineDetailsRepository _repository;

        public OrderVaccineDetailsService(IUnitOfWork unitOfWork, IOrderVaccineDetailsRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<OrderVaccineDetailsResponseDTO>> GetAllOrderVaccineDetailsAsync()
        {
            var details = await _repository.GetAllAsync();
            return details.Select(d => new OrderVaccineDetailsResponseDTO
            {
                Id = d.Id,
                OrderId = d.OrderId,
                VaccineId = d.VaccineId,
                Quantity = d.Quantity,
                TotalPrice = d.TotalPrice
            }).ToList();
        }

        public async Task<OrderVaccineDetailsResponseDTO?> GetOrderVaccineDetailsByIdAsync(string id)
        {
            var details = await _repository.GetByIdAsync(id);
            if (details == null) return null;
            return new OrderVaccineDetailsResponseDTO
            {
                Id = details.Id,
                OrderId = details.OrderId,
                VaccineId = details.VaccineId,
                Quantity = details.Quantity,
                TotalPrice = details.TotalPrice
            };
        }

        public async Task AddOrderVaccineDetailsAsync(OrderVaccineDetailsRequestDTO detailsDto)
        {
            var details = new OrderVaccineDetails
            {
                OrderId = detailsDto.OrderId,
                VaccineId = detailsDto.VaccineId,
                Quantity = detailsDto.Quantity,
                TotalPrice = detailsDto.TotalPrice
            };
            await _repository.InsertAsync(details);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateOrderVaccineDetailsAsync(string id, OrderVaccineDetailsRequestDTO detailsDto)
        {
            var existingDetails = await _repository.GetByIdAsync(id);
            if (existingDetails == null)
                throw new Exception("Order vaccine details not found.");

            existingDetails.OrderId = detailsDto.OrderId;
            existingDetails.VaccineId = detailsDto.VaccineId;
            existingDetails.Quantity = detailsDto.Quantity;
            existingDetails.TotalPrice = detailsDto.TotalPrice;
            await _repository.UpdateAsync(existingDetails);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteOrderVaccineDetailsAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
