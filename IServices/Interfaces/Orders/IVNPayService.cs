using Microsoft.AspNetCore.Http;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.VNPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Orders
{
    public interface IVNPayService
    {
        Task<VNPayPaymentResponseDTO> CreatePaymentUrlAsync(VNPayPaymentRequestDTO request);
        Task<VNPayReturnResponseDTO> HandlePaymentReturnAsync(IQueryCollection query);
    }
}
