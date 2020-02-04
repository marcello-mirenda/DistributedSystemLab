using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace InvoicingFuncApp
{
    public static class StaleStatusFunc
    {
        [FunctionName("StaleStatusFunc")]
        public static void Run(
            [QueueTrigger("StaleStatus", Connection = "StorageConnection")]
            CloudQueueMessage message,
            [CosmosDB(
                databaseName: "Invoicing",
                collectionName: "StaleStatuses",
                ConnectionStringSetting = "cosmosdb")]
                out dynamic document,
            ILogger log)
        {
            var status = JsonConvert.DeserializeObject<StaleStatus>(message.AsString);
            document = new
            {
                id = status.Id,
                status.PartitionKey,
                status.Status
            };
        }
    }
}
