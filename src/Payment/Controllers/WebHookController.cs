using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Payment.Models;
using Payment.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Payment.Controllers
{
    namespace Payment.Controllers
    {
        [ApiController]
        [Route("api/webhook")]
        public class WebhookController : ControllerBase
        {
            private readonly HttpClient _httpClient;
            private readonly string _accessToken;
            private readonly PaymentService _paymentService;

            public WebhookController(IHttpClientFactory httpClientFactory, IConfiguration configuration, PaymentService paymentService)
            {
                _httpClient = httpClientFactory.CreateClient();
                _accessToken = configuration["MercadoPago:AccessToken"];
                _paymentService = paymentService;
            }

            [HttpPost]
            public async Task<IActionResult> ReceiveNotification([FromBody] JsonElement payload)
            {


                // Log para depuración (puedes usar Serilog o simplemente Console)
                Console.WriteLine($"Webhook recibido: {payload}");

                // Extraer información del JSON enviado por Mercado Pago
                if (payload.TryGetProperty("action", out var action) && action.GetString() == "payment.created")
                {
                    if (payload.TryGetProperty("data", out var data) &&
                        data.TryGetProperty("id", out var paymentId))
                    {
                        string paymentIdString = paymentId.GetString();
                        Console.WriteLine($"Nuevo pago recibido. ID: {paymentId}");

                        // Aquí puedes consultar Mercado Pago con el ID del pago
                        var paymentDetails = await GetPaymentDetails(paymentIdString);

                        await _paymentService.SavePaymentAsync(paymentDetails.Amount, paymentDetails.PaymentStatus, paymentDetails.PaymentMethod, paymentDetails.CreatedAt, paymentDetails.MercadoPagoPaymentId);
                    }
                }

                // Responder con 200 OK para que Mercado Pago no vuelva a enviar la notificación
                return Ok();
            }

            private async Task<Payments> GetPaymentDetails(string paymentId)
            {
                var requestUrl = $"https://api.mercadopago.com/v1/payments/{paymentId}?access_token={_accessToken}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var paymentDetails = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    if (paymentDetails.TryGetProperty("status", out var status) &&
                     paymentDetails.TryGetProperty("payment_type_id", out var paymentTypeId) &&
                     paymentDetails.TryGetProperty("additional_info", out var additionalInfo) &&
                     additionalInfo.TryGetProperty("items", out var items) &&
                     items[0].TryGetProperty("unit_price", out var unitPrice) &&
                     paymentDetails.TryGetProperty("date_created", out var dateCreated) &&
                     paymentDetails.TryGetProperty("id", out var id))
                    {



                        var amountValue = unitPrice.GetString();
                        var statusValue = status.GetString();
                        var paymentTypeIdValue = paymentTypeId.GetString();
                        var dateCreatedValue = DateTime.Parse(dateCreated.GetString());
                        var idMP = id.GetInt64();


                        var payment = new Payments
                        {
                            Amount = amountValue,
                            PaymentStatus = statusValue,
                            PaymentMethod = paymentTypeIdValue,
                            CreatedAt = dateCreatedValue,
                            MercadoPagoPaymentId = idMP
                        };


                        Console.WriteLine($"Pago recibido: {payment.MercadoPagoPaymentId}, Estado: {payment.PaymentStatus}, Tipo de Pago: {payment.PaymentMethod}, Precio: {payment.Amount}, Fecha: {payment.CreatedAt}");

                        return payment;
                    }

                }
                return null;
            }
        }
    }
}
