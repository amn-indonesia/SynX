using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core
{
    public class SyncLogDbContext : DbContext
    {
        public SyncLogDbContext(DbContextOptions<SyncLogDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(180);
        }

        public DbSet<SyncLog> SyncLogs { get; set; }
        public DbSet<IdNoCounter> idNoCounters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SyncLog>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<IdNoCounter>()
                .HasKey(e => e.Id);
        }
    }
}
