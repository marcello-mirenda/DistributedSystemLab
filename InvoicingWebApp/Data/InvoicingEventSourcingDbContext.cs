using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoicingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingWebApp.Data
{
    public class InvoicingEventSourcingDbContext : DbContext
    {
        public InvoicingEventSourcingDbContext(DbContextOptions<InvoicingEventSourcingDbContext> options) : base(options)
        {

        }

        public DbSet<InvoiceEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceEvent>()
                .ToContainer("InvoicingEvents")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            modelBuilder.Entity<Invoice>().Property(x => x.Id).ToJsonProperty("id");
        }
    }
}
