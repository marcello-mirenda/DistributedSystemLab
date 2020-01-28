using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InvoicingDataCreator
{
    public class InvoicingContext : DbContext
    {
        private readonly IConfigurationRoot configurationRoot;
        public InvoicingContext(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
        }

        public DbSet<Model.Invoice> Invoices { get; set; }
        public DbSet<Model.InvoiceLine> InvoiceLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseCosmos(
                    this.configurationRoot["AccountEndpoint"],
                    this.configurationRoot["AccountKey"],
                    databaseName: this.configurationRoot["DatabaseNameInvoicing"]);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Invoice>()
                .ToContainer("Invoices")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            //modelBuilder.Entity<Model.Invoice>().Property(x => x.Id).ToJsonProperty("id");
            modelBuilder.Entity<Model.Invoice>().HasMany(x => x.Lines);

            modelBuilder.Entity<Model.InvoiceLine>()
                .ToContainer("InvoiceLines")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            modelBuilder.Entity<Model.InvoiceLine>().HasOne(x => x.Invoice);
            //modelBuilder.Entity<Model.InvoiceLine>().Property(x => x.Id).ToJsonProperty("id");
        }
    }
}
