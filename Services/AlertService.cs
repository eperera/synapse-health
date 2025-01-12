using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Synapse.Services
{
    public class AlertService
    {
        private readonly ILogger<AlertService> _logger;

        private readonly HttpClient _httpClient;

        public AlertService(ILogger<AlertService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public virtual async Task SendAlertMessage(JToken item, string orderId)
        {
            string alertApiUrl = "https://alert-api.com/alerts";
            var alertData = new
            {
                Message = $"Alert for delivered item: Order {orderId}, Item: {item["Description"]}, " +
                            $"Delivery Notifications: {item["deliveryNotification"]}"
            };
            var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(alertApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Alert sent for delivered item: {item["Description"]}");
            }
            else
            {
                _logger.LogError($"Failed to send alert for delivered item: {item["Description"]}");
            }
        }
    }
}
