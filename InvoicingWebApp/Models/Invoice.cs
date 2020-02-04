using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InvoicingWebApp.Models
{
    public class Invoice
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Customer { get; set; }

        public decimal Total { get; set; }

        public bool Deleted { get; set; }

        public string PartitionKey { get; set; }

    }
}
