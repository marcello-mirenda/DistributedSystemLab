using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InvoicingFuncApp
{
    public static class InvoicingEventSourcing
    {
        [FunctionName("InvoicingEventSourcing")]
        public static async Task Run(
            [CosmosDBTrigger(
                databaseName: "InvoicingEventSourcing",
                collectionName: "InvoicingEvents",
                ConnectionStringSetting = "cosmosdb",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true,
                LeaseCollectionPrefix = "Invoicing")]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: null,
                collectionName: null,
                ConnectionStringSetting = "cosmosdb")]
                DocumentClient client,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                var db = await client.GetDatabaseAccountAsync();
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
