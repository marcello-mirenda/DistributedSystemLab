using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InvoicingDataCreator
{
    public class InvoicingEventSourcingContext : DbContext
    {
        private readonly IConfigurationRoot configurationRoot;
        public InvoicingEventSourcingContext(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
        }

        public DbSet<EventSourcingModel.InvoiceEvent> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseCosmos(
                    this.configurationRoot["AccountEndpoint"],
                    this.configurationRoot["AccountKey"],
                    databaseName: this.configurationRoot["DatabaseNameInvoicingEventSourcing"]);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventSourcingModel.InvoiceEvent>()
                .ToContainer("InvoicingEvents")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            modelBuilder.Entity<EventSourcingModel.InvoiceEvent>().Property(x => x.Timestamp).ToJsonProperty("_ts");
        }
    }

}
