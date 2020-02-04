using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InvoicingWebApp.Models
{
    public class StaleStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string PartitionKey { get; set; }
    }
}
