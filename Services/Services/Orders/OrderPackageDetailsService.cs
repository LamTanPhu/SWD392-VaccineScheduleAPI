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
    public class OrderPackageDetailsService : IOrderPackageDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderPackageDetailsRepository _repository;

        public OrderPackageDetailsService(IUnitOfWork unitOfWork, IOrderPackageDetailsRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<OrderPackageDetailsResponseDTO>> GetAllOrderPackageDetailsAsync()
        {
            var details = await _repository.GetAllAsync();
            return details.Select(d => new OrderPackageDetailsResponseDTO
            {
                Id = d.Id,
                OrderId = d.OrderId,
                VaccinePackageId = d.VaccinePackageId,
                Quantity = d.Quantity,
                TotalPrice = d.TotalPrice
            }).ToList();
        }

        public async Task<OrderPackageDetailsResponseDTO?> GetOrderPackageDetailsByIdAsync(string id)
        {
            var details = await _repository.GetByIdAsync(id);
            if (details == null) return null;
            return new OrderPackageDetailsResponseDTO
            {
                Id = details.Id,
                OrderId = details.OrderId,
                VaccinePackageId = details.VaccinePackageId,
                Quantity = details.Quantity,
                TotalPrice = details.TotalPrice
            };
        }

        public async Task AddOrderPackageDetailsAsync(OrderPackageDetailsRequestDTO detailsDto)
        {
            var details = new OrderPackageDetails
            {
                OrderId = detailsDto.OrderId,
                VaccinePackageId = detailsDto.VaccinePackageId,
                Quantity = detailsDto.Quantity,
                TotalPrice = detailsDto.TotalPrice
            };
            await _repository.InsertAsync(details);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateOrderPackageDetailsAsync(string id, OrderPackageDetailsRequestDTO detailsDto)
        {
            var existingDetails = await _repository.GetByIdAsync(id);
            if (existingDetails == null)
                throw new Exception("Order package details not found.");

            existingDetails.OrderId = detailsDto.OrderId;
            existingDetails.VaccinePackageId = detailsDto.VaccinePackageId;
            existingDetails.Quantity = detailsDto.Quantity;
            existingDetails.TotalPrice = detailsDto.TotalPrice;
            await _repository.UpdateAsync(existingDetails);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteOrderPackageDetailsAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
