using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InvoicingWebApp.Models
{
    public class InvoiceEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string ObjectId { get; set; }

        public DateTime Moment { get; set; }

        [JsonProperty("_ts")]
        public int Timestamp { get; set; }

        public string Type { get; set; }

        public string PartitionKey { get; set; }

    }
}
