using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoicingFuncApp.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InvoicingFuncApp
{
    public static class ProjectorFunc
    {
        [FunctionName("ProjectorFunc")]
        [return: Queue("StaleStatus",
                Connection = "StorageConnection")]
        public static async Task<StaleStatus> Run(
            [CosmosDBTrigger(
                databaseName: "InvoicingEventSourcing",
                collectionName: "InvoicingEvents",
                ConnectionStringSetting = "cosmosdb",
                LeaseCollectionName = "Leases",
                CreateLeaseCollectionIfNotExists = true,
                LeaseCollectionPrefix = "Invoicing",
                StartFromBeginning = true,
                CheckpointInterval = 1000)] IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: null,
                collectionName: null,
                ConnectionStringSetting = "cosmosdb")]
                DocumentClient client,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                var projector = new Projector(client, log);
                foreach (var item in input)
                {
                    //try
                    //{
                        await projector.PerformAsync(item.GetPropertyValue<string>("PartitionKey"), item.GetPropertyValue<string>("ObjectId"));
                        return new StaleStatus
                        {
                            Id = "5a46238b-cde6-4369-81c3-9802788b0656",
                            PartitionKey = "Invoicing",
                            Status = "Updated"
                        };
                    //}
                    //catch(Exception ex)
                    //{
                    //    log.LogError(ex, "The function should take countermeasures.");
                    //    return new StaleStatus
                    //    {
                    //        Id = "5a46238b-cde6-4369-81c3-9802788b0656",
                    //        PartitionKey = "Invoicing",
                    //        Status = "Error"
                    //    };
                    //}
                }
            }
            return null;
        }
    }
}
