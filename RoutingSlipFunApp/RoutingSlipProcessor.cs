using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using RoutingSlipFunApp.Models;

namespace RoutingSlipFunApp
{
    public static class RoutingSlipProcessor
    {
        public static async Task<int> Process(RoutingSlip<Order> routingSlip, CloudQueue queue)
        {
            var client = queue.ServiceClient;
            var index = routingSlip.Steps.FindIndex(x => x.Name == queue.Name);
            routingSlip.Steps[index].Completed = true;
            var nextIndex = index + 1;
            if (routingSlip.Steps.Count > nextIndex)
            {
                var nextQueue = client.GetQueueReference(routingSlip.Steps[nextIndex].Name);
                await nextQueue.CreateIfNotExistsAsync();
                var message = new CloudQueueMessage(null);
                message.SetMessageContent(JsonConvert.SerializeObject(routingSlip));
                await nextQueue.AddMessageAsync(message);
            }
            return index;
        }

    }
}
