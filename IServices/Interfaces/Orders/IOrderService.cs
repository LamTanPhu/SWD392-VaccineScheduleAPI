using ModelViews.Requests;
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
        Task<OrderResponseDTO> CreateOrderAsync(OrderRequestDTO orderDto);
        Task<OrderResponseDTO> AddOrderDetailsAsync(AddOrderDetailsRequestDTO request);
        Task<OrderResponseDTO> RemoveOrderDetailsAsync(RemoveOrderDetailsRequestDTO request);
        Task<OrderResponseDTO> SetPayLaterAsync(PayLaterRequestDTO request);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByParentIdAsync(string parentId);


    }
}
