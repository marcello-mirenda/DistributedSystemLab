using System;
using System.Collections.Generic;
using System.Text;

namespace InvoicingDataCreator.Model
{
    public class Invoice
    {
        public string Id { get; set; }

        public string Customer { get; set; }

        public decimal Total { get; set; }

        public bool Deleted { get; set; }

        public string PartitionKey { get; set; }

        public virtual ICollection<InvoiceLine> Lines { get; set; }
    }
}
