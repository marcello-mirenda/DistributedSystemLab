using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using RoutingSlipWebApp.Models;

namespace RoutingSlipWebApp.Services
{
    public class NewOrderService : IService
    {
        private readonly Order _order;

        public NewOrderService(Order order)
        {
            _order = order;
        }

        public async Task RunAsync()
        {
            var routingSlip = new RoutingSlip<Order>
            {
                Steps = new List<Step>
                {
                    new Step { Name = "new-order-step-a", Completed = false },
                    new Step { Name = "new-order-step-b", Completed = false },
                    new Step { Name = "new-order-step-c", Completed = false },
                    new Step { Name = "new-order-result", Completed = false },
                },
                Data = _order
            };

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(routingSlip.Steps[0].Name);
            await queue.CreateIfNotExistsAsync();
            var message = new CloudQueueMessage(null);
            message.SetMessageContent(JsonConvert.SerializeObject(routingSlip));
            await queue.AddMessageAsync(message);
        }
    }
}
