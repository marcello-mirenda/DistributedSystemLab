using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InvoicingDataCreator.EventSourcingModel
{
    public class InvoiceEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string ObjectId { get; set; }

        public DateTime When { get; set; }

        [JsonProperty("_ts")]
        public int Timestamp { get; set; }

        public string Type { get; set; }

        public string PartitionKey { get; set; }
    }
}
