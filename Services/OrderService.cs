using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Synapse.Services
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;

        private readonly HttpClient _httpClient;

        public OrderService(ILogger<OrderService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public virtual async Task<JObject[]> FetchMedicalEquipmentOrders()
        {
            string ordersApiUrl = "https://orders-api.com/orders";
            var response = await _httpClient.GetAsync(ordersApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var ordersData = await response.Content.ReadAsStringAsync();
                return JArray.Parse(ordersData).ToObject<JObject[]>() ?? new JObject[0];
            }
            else
            {
                _logger.LogError("Failed to fetch orders from API.");
                return new JObject[0];
            }
        }

        public async Task UpdateOrder(JObject order)
        {
            string updateApiUrl = "https://update-api.com/update";
            var content = new StringContent(order.ToString(), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(updateApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Updated order sent for processing: OrderId {order["OrderId"]}");
            }
            else
            {
                _logger.LogError($"Failed to send updated order for processing: OrderId {order["OrderId"]}");
            }
        }
    }
}
