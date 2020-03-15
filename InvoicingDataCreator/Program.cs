using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace InvoicingDataCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                  .AddJsonFile("appSettings.json")
                  .Build();

            Console.WriteLine();
            Console.WriteLine("Getting started with Cosmos:");
            Console.WriteLine();

            // await IntializeInvoicingContextAsync(configuration);

            await InitializeInvoicingEventSourcingContext(configuration);
        }

        private static async Task InitializeInvoicingEventSourcingContext(IConfigurationRoot configuration)
        {
            using (var context = new InvoicingEventSourcingContext(configuration))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var today = DateTime.Today;

                var partitionKey = $"{today.Year}-{today.Month}";

                var invoiceEvent = new EventSourcingModel.InvoiceEvent
                {
                    Id = "1",
                    PartitionKey = partitionKey,
                    Type = "InvoiceChanged",
                    Moment = DateTime.UtcNow,
                    ObjectId = "1"
                };
                
                var client = context.Database.GetCosmosClient();
                var esdb = client.GetDatabase("InvoicingEventSourcing");
                var esc = esdb.GetContainer("InvoicingEvents");
                var jObject = JObject.FromObject(invoiceEvent);
                jObject["Data"] = JObject.Parse("{\"Customer\": \"AAA Asso spa changed\"}");
                await esc.CreateItemAsync(jObject);

                await client.CreateDatabaseIfNotExistsAsync("Invoicing");
                var invdb = client.GetDatabase("Invoicing");
                await invdb.CreateContainerIfNotExistsAsync("Invoices", "/PartitionKey");
                await invdb.CreateContainerIfNotExistsAsync("StaleStatuses", "/PartitionKey");
                var staleStatuses = invdb.GetContainer("StaleStatuses");
                await staleStatuses.CreateItemAsync(JObject.FromObject(new
                {
                    id = "5a46238b-cde6-4369-81c3-9802788b0656",
                    PartitionKey = "Invoicing",
                    Status = "Updated"
                }));
            }
        }

        private static async Task IntializeInvoicingContextAsync(IConfigurationRoot configuration)
        {
            using (var context = new InvoicingContext(configuration))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var id = "1";
                var today = DateTime.Today;
                var partitionKey = $"{today.Year}-{today.Month}";
                context.Add(new Model.Invoice
                {
                    Id = id,
                    PartitionKey = partitionKey,
                    Customer = "AAA Asso spa",
                    Deleted = false,
                    Total = 100,
                    Lines = new List<Model.InvoiceLine>()
                    {
                        new Model.InvoiceLine
                        {
                            Id = $"{id}-1",
                            Description = "Item #1",
                            PartitionKey = $"{partitionKey}-{id}",
                            Price = 100,
                            Quantity = 1
                        }
                    }
                });

                await context.SaveChangesAsync();
            }

            using (var context = new InvoicingContext(configuration))
            {
                var invoice = await context.Invoices.FirstAsync();
                await context.Entry(invoice).Collection(x => x.Lines).LoadAsync();
                Console.WriteLine($"First invoice will ship to: {invoice.Customer}, {invoice.Lines.First().Description}");
                Console.WriteLine();
            }
        }
    }
}
