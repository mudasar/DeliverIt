using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DeliverIt.Models;

namespace DeliverIt.Data
{
    public class DeliverItContext : DbContext
    {
        public DeliverItContext(DbContextOptions<DeliverItContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(s => s.Email).IsUnique();
            modelBuilder.Entity<Partner>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Delivery>().HasIndex(s => s.OrderId).IsUnique();

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
    }
}
