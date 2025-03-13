using Microsoft.AspNetCore.Http;
using ModelViews.Requests.Order;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.Order;
using ModelViews.Responses.Payment;
using ModelViews.Responses.VNPay;
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
        Task UpdatePaymentDetailsAsync(string name, PaymentDetailsResponseDTO detailsDto);
        Task<VNPayPaymentResponseDTO> CreatePaymentUrlAsync(VNPayPaymentRequestDTO request);
        Task<VNPayReturnResponseDTO> HandlePaymentReturnAsync(IQueryCollection query);
        Task<byte[]> CreateQRCodeAsync(VNPayPaymentRequestDTO request);
    }
}
