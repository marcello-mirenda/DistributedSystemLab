using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using RoutingSlipFunApp.Models;

namespace RoutingSlipFunApp
{
    public static class NewOrderStepC
    {
        [FunctionName("new-order-step-c")]
        public static async Task Run(
            [QueueTrigger("new-order-step-c", Connection = "QueueStorage")]
            RoutingSlip<Order> routingSlip,
            [Queue("new-order-step-c", Connection = "QueueStorage")]
            CloudQueue queue,
            ILogger log)
        {
            int index = await RoutingSlipProcessor.Process(routingSlip, queue);

            log.LogInformation($"C# Queue trigger function processed: {routingSlip.Steps[index].Name}");
        }

    }
}
