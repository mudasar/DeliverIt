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

        public DbSet<User> Users { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
    }
}
