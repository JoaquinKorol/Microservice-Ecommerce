using Core.Exceptions;
using Core.Interfaces;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.OpenApi.Writers;
using Payment.Models;
using Payment.Repositories;
using System.Runtime.CompilerServices;

namespace Payment.Services
{
    public class PaymentService
    {
        private readonly IRepository<Payments> _repository;

        public PaymentService(IRepository<Payments> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Payments>> GetPaymentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Payments> GetPaymentByIdAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new NotFoundException($"Payment with ID {id} not found.");
            }
            return payment;
        }

        public async Task<string> CreatePaymentAsync(decimal amount, string description, int orderId)
        {
            var request = new PreferenceRequest
            {
                NotificationUrl = "https://baa6-2800-21c1-c000-82d-283b-e168-6e98-240e.ngrok-free.app/api/webhook",
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = description,
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = amount,
                    }

                }
            };


            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            return preference.InitPoint;
        }

        public async Task<Payments> SavePaymentAsync(string amount, string status, string paymentMethod, DateTime createdAt, long paymentId)
        {
            var payment = new Payments
            {
                OrderId = 123,
                Amount = amount,
                PaymentStatus = status,
                PaymentMethod = paymentMethod,
                CreatedAt = createdAt,
                MercadoPagoPaymentId = paymentId
            };

            await _repository.AddAsync(payment);
            return payment;

        }
    }
}
    


