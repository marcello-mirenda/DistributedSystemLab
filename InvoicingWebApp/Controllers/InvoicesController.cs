using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using InvoicingWebApp.Data;
using InvoicingWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace InvoicingWebApp.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly InvoicingDbContext _context;
        private readonly InvoicingEventSourcingDbContext _contextEvents;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(InvoicingDbContext context, InvoicingEventSourcingDbContext contextEvents, ILogger<InvoicesController> logger)
        {
            _context = context;
            _contextEvents = contextEvents;
            _logger = logger;
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AggregateId,Customer,Total,Deleted,PartitionKey")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var status = await _context.StaleStatuses.SingleAsync(x => x.Id == "5a46238b-cde6-4369-81c3-9802788b0656");
                status.Status = "Stale";
                await _context.SaveChangesAsync();

                invoice.PartitionKey = $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}";
                invoice.AggregateId = Guid.NewGuid().ToString();
                invoice.PartitionKey = $"{DateTime.Today.Year}-{DateTime.Today.Month}";
                await CreateEventAsync(invoice, "InvoiceCreated");
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.AggregateId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/CheckOutAll
        public async Task<IActionResult> CheckOutAll()
        {
            var client = _contextEvents.Database.GetCosmosClient();
            var query = new QueryDefinition("SELECT c.ObjectId FROM c GROUP BY c.ObjectId");
            var moment = DateTime.Today;
            var evs = client.GetContainer("InvoicingEventSourcing", "InvoicingEvents");
            var resultSetIterator = evs.GetItemQueryIterator<InvoiceEvent>(query, requestOptions: new QueryRequestOptions() { PartitionKey = new PartitionKey($"{moment.Year}-{moment.Month}") });
            var results = new List<InvoiceEvent>();
            while (resultSetIterator.HasMoreResults)
            {
                var response = await resultSetIterator.ReadNextAsync();
                results.AddRange(response);
                if (response.Diagnostics != null)
                {
                    _logger.LogInformation($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics.ToString()}");
                }
            }
            foreach (var item in results)
            {
                await CreateEventAsync(new
                {
                    AggregateId = item.ObjectId
                }, "InvoiceCheckedOut");
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.AggregateId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AggregateId,Customer,Total,Deleted,PartitionKey")] Invoice invoice)
        {
            if (id != invoice.AggregateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentInvoice = await _context.Invoices.FindAsync(id);
                if (currentInvoice == null)
                {
                    return NotFound();
                }
                var status = await _context.StaleStatuses.SingleAsync(x => x.Id == "5a46238b-cde6-4369-81c3-9802788b0656");
                status.Status = "Stale";
                await _context.SaveChangesAsync();

                var data = Diff(invoice, currentInvoice);
                await CreateEventAsync(data, "InvoiceChanged");
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        // GET: Invoices/CheckOut/5
        public async Task<IActionResult> CheckOut(string id)
        {
            var currentInvoice = await _context.Invoices.FindAsync(id);
            if (currentInvoice == null)
            {
                return NotFound();
            }

            var data = Diff(currentInvoice, currentInvoice);
            await CreateEventAsync(data, "InvoiceCheckedOut");
            return RedirectToAction(nameof(Index));
        }


        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var status = await _context.StaleStatuses.SingleAsync(x => x.Id == "5a46238b-cde6-4369-81c3-9802788b0656");
            return View(new InvoiceViewModel
            {
                Invoices = await _context.Invoices.ToListAsync(),
                Status = status
            });
        }

        private static dynamic Diff(Invoice newItem, Invoice currentItem)
        {
            var dictionaryNew = (IDictionary<string, object>)ToExpandoObject(newItem);
            var dictionaryCurrent = (IDictionary<string, object>)ToExpandoObject(currentItem);
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>;

            foreach (var pair in dictionaryNew)
            {
                if (pair.Key != "AggregateId" && dictionaryCurrent[pair.Key].Equals(pair.Value))
                {
                    continue;
                }
                d[pair.Key] = pair.Value;
            }

            return result;
        }

        private static ExpandoObject ToExpandoObject(object obj)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var property in obj.GetType().GetProperties())
                dictionary.Add(property.Name, property.GetValue(obj));
            return expando;
        }

        private async Task CreateEventAsync(dynamic data, string type)
        {
            var moment = DateTime.UtcNow;

            var partitionKey = $"{moment.Year}-{moment.Month}";

            var invoiceEvent = new InvoiceEvent
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = partitionKey,
                Type = type,
                Moment = moment,
                ObjectId = data.AggregateId
            };

            var client = _contextEvents.Database.GetCosmosClient();
            var database = client.GetDatabase("InvoicingEventSourcing");
            var container = database.GetContainer("InvoicingEvents");
            var jObject = JObject.FromObject(invoiceEvent);
            jObject["Data"] = JObject.FromObject(data);
            await container.CreateItemAsync(jObject);
        }

    }
}
