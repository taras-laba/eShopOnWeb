using Microsoft.eShopWeb.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Microsoft.eShopWeb.Web.Services
{
    public class OrderReserverService: IOrderReserverService
    {
        IConfiguration _configuration;
        public OrderReserverService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PlaceOrderAsync(Dictionary<string, int> items)
        {
            var order = new { Items = items.Select(a => new { Id = a.Key, Quantity = a.Value }) };

            string serviceBusConnectionString = _configuration["OrderReserverServiceBusConnectionString"];
            string queueName = _configuration["OrderReserverQueueName"];
            IQueueClient queueClient = new QueueClient(serviceBusConnectionString, queueName);

            try
            {
                
                string messageBody = JsonConvert.SerializeObject(order);
                Console.WriteLine($"Sending: {messageBody}");
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                await queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

            await queueClient.CloseAsync();
        }
    }
}
