using System;
using System.Collections.Generic;
using System.Text;

namespace InvoicingDataCreator.Model
{
    public class InvoiceLine
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public string PartitionKey { get; set; }

        public Invoice Invoice { get; set; }

        public string InvoiceId { get; set; }
    }
}
