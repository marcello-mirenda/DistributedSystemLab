using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InvoicingWebApp.Models
{
    public class Invoice
    {
        [JsonProperty("id")]
        public string AggregateId { get; set; }

        public string Customer { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public decimal Total { get; set; }

        public bool Deleted { get; set; }

        public string PartitionKey { get; set; }

    }
}
