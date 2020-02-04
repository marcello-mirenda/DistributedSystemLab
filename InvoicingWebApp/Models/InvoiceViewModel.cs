using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoicingWebApp.Models
{
    public class InvoiceViewModel
    {
        public IEnumerable<Invoice> Invoices { get; set; }

        public StaleStatus Status { get; set; }
    }
}
