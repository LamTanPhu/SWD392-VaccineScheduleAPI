using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Orders
{
    public interface IOrderPackageDetailsService
    {
        Task<IEnumerable<OrderPackageDetailsResponseDTO>> GetAllOrderPackageDetailsAsync();
        Task<OrderPackageDetailsResponseDTO?> GetOrderPackageDetailsByIdAsync(string id);
        Task AddOrderPackageDetailsAsync(OrderPackageDetailsRequestDTO details);
        Task UpdateOrderPackageDetailsAsync(string id, OrderPackageDetailsRequestDTO detailsDto);
        Task DeleteOrderPackageDetailsAsync(string id);
    }
}
