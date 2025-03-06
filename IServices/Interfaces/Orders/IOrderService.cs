using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync();
        Task<OrderResponseDTO?> GetOrderByIdAsync(string id);
        Task AddOrderAsync(OrderRequestDTO order);
        Task UpdateOrderAsync(string id, OrderRequestDTO orderDto);
        Task DeleteOrderAsync(string id);
    }
}
