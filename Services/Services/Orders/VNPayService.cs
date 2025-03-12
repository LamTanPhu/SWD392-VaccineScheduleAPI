using IRepositories.IRepository;
using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ModelViews.Config;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.VNPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Orders
{
    public class VNPayService : IVNPayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VNPayConfig _config;

        public VNPayService(IUnitOfWork unitOfWork, IOptions<VNPayConfig> config)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<VNPayPaymentResponseDTO> CreatePaymentUrlAsync(VNPayPaymentRequestDTO request)
        {
            if (request == null || request.Amount <= 0 || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Invalid payment request data.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", _config.Version }, // Dùng từ config
            { "vnp_Command", _config.Command }, // Dùng từ config
            { "vnp_TmnCode", _config.TmnCode },
            { "vnp_Amount", ((int)(request.Amount * 100)).ToString() },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", _config.CurrCode }, // Dùng từ config
            { "vnp_IpAddr", "127.0.0.1" },
            { "vnp_Locale", _config.Locale }, // Dùng từ config
            { "vnp_OrderInfo", request.OrderInfo },
            { "vnp_OrderType", "250000" },
            { "vnp_ReturnUrl", _config.ReturnUrl },
            { "vnp_TxnRef", request.OrderId },
        };

                string queryString = BuildQueryString(vnpParams);
                string secureHash = HmacSHA512(_config.HashSecret, queryString);
                vnpParams["vnp_SecureHash"] = secureHash;
                string paymentUrl = _config.BaseUrl + "?" + BuildQueryString(vnpParams); // Dùng BaseUrl
                await _unitOfWork.CommitTransactionAsync();
                return new VNPayPaymentResponseDTO { PaymentUrl = paymentUrl };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create VNPay payment URL: {ex.Message}");
            }
        }


        public async Task<VNPayReturnResponseDTO> HandlePaymentReturnAsync(IQueryCollection query)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vnpParams = query.ToDictionary(k => k.Key, v => v.Value.ToString());
                string secureHash = vnpParams["vnp_SecureHash"];
                vnpParams.Remove("vnp_SecureHash");

                string computedHash = HmacSHA512(_config.HashSecret, BuildQueryString(vnpParams));
                bool isValid = secureHash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

                var response = new VNPayReturnResponseDTO
                {
                    TransactionId = vnpParams["vnp_TransactionNo"],
                    OrderId = vnpParams["vnp_TxnRef"],
                    Amount = decimal.Parse(vnpParams["vnp_Amount"]) / 100, // VND / 100 
                    ResponseCode = vnpParams["vnp_ResponseCode"],
                    IsSuccess = isValid && vnpParams["vnp_ResponseCode"] == "00"
                };

                if (!response.IsSuccess)
                    throw new Exception("Payment verification failed or transaction unsuccessful.");

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to handle VNPay return: {ex.Message}");
            }
        }

        private string BuildQueryString(Dictionary<string, string> parameters)
        {
            var sortedParams = parameters.OrderBy(p => p.Key);
            return string.Join("&", sortedParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
        }

        private string HmacSHA512(string key, string input)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
