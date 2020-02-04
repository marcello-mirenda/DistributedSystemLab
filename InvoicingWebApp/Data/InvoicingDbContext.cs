using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoicingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingWebApp.Data
{
    public class InvoicingDbContext : DbContext
    {
        public InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : base(options)
        {

        }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<StaleStatus> StaleStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>()
                .ToContainer("Invoices")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            modelBuilder.Entity<Invoice>().Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<StaleStatus>()
                .ToContainer("StaleStatuses")
                .HasNoDiscriminator()
                .HasPartitionKey(x => x.PartitionKey);
            modelBuilder.Entity<StaleStatus>().Property(x => x.Id).ToJsonProperty("id");
        }
    }
}
