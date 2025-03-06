using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Orders
{
    public interface IOrderVaccineDetailsService
    {
        Task<IEnumerable<OrderVaccineDetailsResponseDTO>> GetAllOrderVaccineDetailsAsync();
        Task<OrderVaccineDetailsResponseDTO?> GetOrderVaccineDetailsByIdAsync(string id);
        Task AddOrderVaccineDetailsAsync(OrderVaccineDetailsRequestDTO details);
        Task UpdateOrderVaccineDetailsAsync(string id, OrderVaccineDetailsRequestDTO detailsDto);
        Task DeleteOrderVaccineDetailsAsync(string id);
    }
}
