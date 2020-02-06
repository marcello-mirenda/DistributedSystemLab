using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace InvoicingFuncApp
{
    public class Projector
    {
        private readonly DocumentClient _documentClient;
        private readonly ILogger _logger;

        public Projector(DocumentClient documentClient, ILogger logger)
        {
            _documentClient = documentClient;
            _logger = logger;
        }

        public async Task PerformAsync(string partitionKey, string objectID)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("InvoicingEventSourcing", "InvoicingEvents");
            var events = _documentClient.CreateDocumentQuery(collectionUri, new SqlQuerySpec
            {
                QueryText = "SELECT * FROM InvoicingEvents i WHERE i.ObjectId = @id ORDER BY i.Moment",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", objectID),
                    }
            },
                new FeedOptions
                {
                    EnableCrossPartitionQuery = false,
                    PartitionKey = new PartitionKey(partitionKey)
                }).ToList();

            var action = "";
            dynamic actionData = null;
            foreach (var item in events)
            {
                _logger.LogInformation("Event ID {EventID} {Type} {ObjectId}", (object)item.id, (object)item.Type, (object)item.ObjectId);
                if (item.Type == "InvoiceChanged")
                {
                    var data = LoadOrCreate(partitionKey, objectID);
                    var doc = data.doc;
                    doc = Merge(doc, item.Data);
                    actionData = doc;
                    action = "Update";
                }
                else if (item.Type == "InvoiceCreated")
                {
                    var data = LoadOrCreate(partitionKey, objectID);
                    if (data.isNew)
                    {
                        var doc = item.Data;
                        ResourceResponse<Document> resp = await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Invoicing", "Invoices"), doc, null, true);
                        _logger.LogInformation("Created document {Id} status {status}", (object)doc.id.ToString(), resp.StatusCode.ToString());
                    }
                    else
                    {
                        _logger.LogInformation("Already created document {Id}", (object)data.doc.id.ToString());
                    }
                }
            }
            if (action == "Update")
            {
                ResourceResponse<Document> resp = await _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("Invoicing", "Invoices", actionData.id), actionData);
                _logger.LogInformation("Updated document {id} status {status}", (object)actionData.id, resp.StatusCode.ToString());
            }
        }

        private (dynamic doc, bool isNew) LoadOrCreate(string partitionKey, string objectID)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("Invoicing", "Invoices");
            var document = _documentClient.CreateDocumentQuery(collectionUri, new SqlQuerySpec
            {
                QueryText = "SELECT * FROM Invoices i WHERE i.id = @id",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", objectID),
                    }
            },
            new FeedOptions
            {
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionKey)
            }).ToList().SingleOrDefault();
            var newDocument = false;
            if (document == null)
            {
                newDocument = true;
            }
            return (document, newDocument);
        }

        private static dynamic Merge(dynamic item1, JObject item2)
        {
            var dictionary1 = (IDictionary<string, object>)item1;
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary

            foreach (var pair in dictionary1)
            {
                d[pair.Key] = pair.Value;
            }
            foreach (var pair in item2)
            {
                d[pair.Key] = pair.Value.ToObject<string>();
            }

            return result;
        }
    }
}
