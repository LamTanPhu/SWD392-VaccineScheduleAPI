using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Orders
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDetailsResponseDTO>> GetAllPaymentDetailsAsync();
        Task<PaymentDetailsResponseDTO?> GetPaymentDetailsByNameAsync(string name);
        Task AddPaymentDetailsAsync(PaymentDetailsResponseDTO details);
        Task UpdatePaymentDetailsAsync(string name, PaymentDetailsResponseDTO detailsDto);
        Task DeletePaymentDetailsAsync(string name);
    }
}
