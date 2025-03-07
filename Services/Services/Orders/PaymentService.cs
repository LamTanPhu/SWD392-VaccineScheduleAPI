using IRepositories.IRepository.Orders;
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

namespace Services.Services.Orders
{
    public class PaymentService : IPaymentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentRepository _repository;

        public PaymentService(IUnitOfWork unitOfWork, IPaymentRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public async Task AddPaymentDetailsAsync(PaymentDetailsResponseDTO details)
        {
            var payment = new Payment
            {
                OrderId = details.OrderId,
                PaymentName = details.PaymentName,
                PaymentMethod = details.PaymentMethod,
                PaymentDate = details.PaymentDate,
                PaymentStatus = details.PaymentStatus,
                PayAmount = details.PayAmount
            };
            await _repository.InsertAsync(payment);
            await _unitOfWork.SaveAsync();
        }

        public Task DeletePaymentDetailsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PaymentDetailsResponseDTO>> GetAllPaymentDetailsAsync()
        {
            var payments = await _repository.GetAllAsync();
            return payments.Select(p => new PaymentDetailsResponseDTO
            {
                OrderId = p.OrderId,
                PaymentName = p.PaymentName,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                PayAmount = p.PayAmount
            }).ToList();
        }

        public async Task<PaymentDetailsResponseDTO?> GetPaymentDetailsByNameAsync(string name)
        {
            var payment = await _repository.GetByPaymentnameAsync(name);
            if (payment == null) return null;
            return new PaymentDetailsResponseDTO
            {
                OrderId = payment.OrderId,
                PaymentName = payment.PaymentName,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                PaymentStatus = payment.PaymentStatus,
                PayAmount = payment.PayAmount
            };
        }

        public Task UpdatePaymentDetailsAsync(string name, PaymentDetailsResponseDTO detailsDto)
        {
            throw new NotImplementedException();
        }
    }
}
