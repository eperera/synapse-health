using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Synapse.Services;

namespace Synapse.OrdersExample
{
    public class SynapseCode
    {
        private readonly ILogger _logger;

        private readonly AlertService _alertService;

        private readonly OrderService _orderService;

        public SynapseCode(ILogger logger, AlertService alertService, OrderService orderService) 
        { 
            _logger = logger;
            _alertService = alertService;
            _orderService = orderService;
        }

        public async Task ProcessOrders()
        {
            var medicalEquipmentOrders = await _orderService.FetchMedicalEquipmentOrders();
            foreach (var order in medicalEquipmentOrders)
            {
                var updatedOrder = ProcessOrder(order).Result;
                _orderService.UpdateOrder(updatedOrder).GetAwaiter().GetResult();
            }

            _logger.LogInformation("Results sent to relevant APIs.");
        }

        private async Task<JObject> ProcessOrder(JObject order)
        {
            var items = order["Items"]?.ToObject<JArray>() ?? new JArray();

            foreach (var item in items)
            {
                if (IsItemDelivered(item))
                {
                    var orderId = order["OrderId"];
                    if (orderId != null) 
                    { 
                        await _alertService.SendAlertMessage(item, orderId.ToString());
                        IncrementDeliveryNotification(item);
                    }
                }
            }

            return order;
        }

        private bool IsItemDelivered(JToken item)
        {
            return item["Status"]?.ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase)?? false;
        }

        private void IncrementDeliveryNotification(JToken item)
        {
            item["DeliveryNotification"] = item["DeliveryNotification"]?.Value<int>() + 1;
        }
    }
}
