using Microsoft.EntityFrameworkCore;
using SynX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Test.UnitTest
{
    public class SyncLogSqliteTest
    {
        public SyncLogSqliteTest(DbContextOptions<SyncLogDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<SyncLogDbContext> ContextOptions { get; }

        private void Seed()
        {
            using(var context = new SyncLogDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.SaveChanges();
            }
        }
    }
}
