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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _repository;

        public OrderService(IUnitOfWork unitOfWork, IOrderRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(o => new OrderResponseDTO
            {
                Id = o.Id,
                ProfileId = o.ProfileId,
                PurchaseDate = o.PurchaseDate,
                TotalAmount = o.TotalAmount,
                TotalOrderPrice = o.TotalOrderPrice,
                Status = o.Status
            }).ToList();
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(string id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return null;
            return new OrderResponseDTO
            {
                Id = order.Id,
                ProfileId = order.ProfileId,
                PurchaseDate = order.PurchaseDate,
                TotalAmount = order.TotalAmount,
                TotalOrderPrice = order.TotalOrderPrice,
                Status = order.Status
            };
        }

        public async Task AddOrderAsync(OrderRequestDTO orderDto)
        {
            var order = new Order
            {
                ProfileId = orderDto.ProfileId,
                PurchaseDate = orderDto.PurchaseDate,
                TotalAmount = orderDto.TotalAmount,
                TotalOrderPrice = orderDto.TotalOrderPrice,
                Status = orderDto.Status
            };
            await _repository.InsertAsync(order);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateOrderAsync(string id, OrderRequestDTO orderDto)
        {
            var existingOrder = await _repository.GetByIdAsync(id);
            if (existingOrder == null)
                throw new Exception("Order not found.");

            existingOrder.PurchaseDate = orderDto.PurchaseDate;
            existingOrder.TotalAmount = orderDto.TotalAmount;
            existingOrder.TotalOrderPrice = orderDto.TotalOrderPrice;
            existingOrder.Status = orderDto.Status;
            await _repository.UpdateAsync(existingOrder);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteOrderAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
