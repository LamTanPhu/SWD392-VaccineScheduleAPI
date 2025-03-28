﻿using IRepositories.IRepository.Orders;
using IRepositories.IRepository;
using IServices.Interfaces.Orders;
using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IRepositories.Entity.Orders;
using Repositories.Repository;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ModelViews.Config;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.VNPay;
using System.Security.Cryptography;
using System.Text;
using ModelViews.Responses.Payment;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using AutoMapper;

namespace Services.Services.Orders
{
    public class PaymentService : IPaymentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly VNPayConfig _config;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper; 

        public PaymentService(
            IUnitOfWork unitOfWork,
            IOptions<VNPayConfig> config,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IMapper mapper) 
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Kiểm tra null
        }

        public async Task<VNPayPaymentResponseDTO> CreatePaymentUrlAsync(VNPayPaymentRequestDTO request)
        {
            if (request == null || request.Amount <= 0 || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Invalid payment request data.");

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
                    Amount = decimal.Parse(vnpParams["vnp_Amount"]) / 100,
                    ResponseCode = vnpParams["vnp_ResponseCode"],
                    IsSuccess = isValid && vnpParams["vnp_ResponseCode"] == "00"
                };

                var payment = new Payment
                {
                    OrderId = response.OrderId,
                    TransactionId = response.TransactionId,
                    PaymentName = "VNPay",
                    PaymentMethod = vnpParams.ContainsKey("vnp_CardType") ? vnpParams["vnp_CardType"] : "VNPay",
                    PaymentDate = DateTime.ParseExact(vnpParams["vnp_PayDate"], "yyyyMMddHHmmss", null),
                    PayAmount = response.Amount,
                    CreatedTime = DateTime.Now,
                    LastUpdatedTime = DateTime.Now
                };

                if (response.IsSuccess)
                {                   
                    //var payment = new Payment
                    //{
                    //    Id = Guid.NewGuid().ToString(), 
                    //    OrderId = response.OrderId,
                    //    TransactionId = response.TransactionId,
                    //    PaymentName = "VNPay",
                    //    PaymentMethod = vnpParams.ContainsKey("vnp_CardType") ? vnpParams["vnp_CardType"] : "VNPay",
                    //    PaymentDate = DateTime.ParseExact(vnpParams["vnp_PayDate"], "yyyyMMddHHmmss", null),
                    //    PaymentStatus = "Success",
                    //    PayAmount = response.Amount,
                    //    CreatedTime = DateTime.Now,
                    //    LastUpdatedTime = DateTime.Now
                    //};
                    payment.PaymentStatus = "Success";

                    // Cập nhật trạng thái Order thành "Paid"
                    var order = await _orderRepository.GetByIdAsync(response.OrderId);
                    if (order == null)
                        throw new Exception($"Order with ID {response.OrderId} not found.");

                    if (order.Status != "Pending" && order.Status != "PayLater")
                        throw new Exception($"Order {response.OrderId} is already in {order.Status} status and cannot be updated to Paid.");

                    order.Status = "Paid";
                    order.LastUpdatedTime = DateTime.Now;
                    await _orderRepository.UpdateAsync(order);
                }
                else
                {
                    payment.PaymentStatus = "Failed";
                }

                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to handle VNPay return: {ex.Message}");
            }
        }

        public async Task<byte[]> CreateQRCodeAsync(VNPayPaymentRequestDTO request)
        {
            if (request == null || request.Amount <= 0 || string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("Invalid payment request data.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vnpParams = new Dictionary<string, string>
                {
                    { "vnp_Version", _config.Version },
                    { "vnp_Command", _config.Command },
                    { "vnp_TmnCode", _config.TmnCode },
                    { "vnp_Amount", ((int)(request.Amount * 100)).ToString() },
                    { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                    { "vnp_CurrCode", _config.CurrCode },
                    { "vnp_IpAddr", "127.0.0.1" },
                    { "vnp_Locale", _config.Locale },
                    { "vnp_OrderInfo", request.OrderInfo },
                    { "vnp_OrderType", "260000" }, // Có thể đổi thành "260000" nếu dành riêng cho QR
                    { "vnp_ReturnUrl", _config.ReturnUrl },
                    { "vnp_TxnRef", request.OrderId },
                };

                string queryString = BuildQueryString(vnpParams);
                string secureHash = HmacSHA512(_config.HashSecret, queryString);
                vnpParams["vnp_SecureHash"] = secureHash;

                // Tạo URL thanh toán từ VNPay
                string paymentUrl = _config.BaseUrl + "?" + BuildQueryString(vnpParams);

                // Tạo mã QR từ URL
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20); 
                //string base64Image = Convert.ToBase64String(qrCodeImage);

                await _unitOfWork.CommitTransactionAsync();
                //return new VNPayQRCodeResponseDTO { QRCodeImageBase64 = base64Image };
                return qrCodeBytes;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create VNPay QR code: {ex.Message}");
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

        public async Task<IEnumerable<PaymentDetailsResponseDTO>> GetAllPaymentDetailsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDetailsResponseDTO>>(payments); 
        }

        public async Task<PaymentDetailsResponseDTO?> GetPaymentDetailsByNameAsync(string name)
        {
            var payment = await _paymentRepository.GetByPaymentnameAsync(name);
            if (payment == null) return null;
            return _mapper.Map<PaymentDetailsResponseDTO>(payment); 
        }

        public async Task<PaymentDetailsResponseDTO?> GetPaymentDetailsByIdAsync(string id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return null;
            return _mapper.Map<PaymentDetailsResponseDTO>(payment); 
        }

        public async Task<PaymentDetailsResponseDTO> PayAtFacilityAsync(PayAtFacilityRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                    throw new Exception("Order không tồn tại.");

                if (order.Status != "PayLater")
                    throw new Exception("Order phải ở trạng thái PayLater để thanh toán tại cơ sở.");


                order.Status = "Paid";
                order.LastUpdatedTime = DateTime.Now;
                await _orderRepository.UpdateAsync(order);


                var payment = new Payment
                {
                    OrderId = order.Id,
                    TransactionId = $"FACILITY-{Guid.NewGuid().ToString()}",
                    PaymentName = "Thanh toán tại cơ sở",
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Success",
                    PayAmount = order.TotalOrderPrice,
                    CreatedTime = DateTime.Now,
                    LastUpdatedTime = DateTime.Now
                };

                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Trả về thông tin Payment bằng AutoMapper
                return _mapper.Map<PaymentDetailsResponseDTO>(payment);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Thanh toán tại cơ sở thất bại: {ex.Message}");
            }
        }


    }

}

