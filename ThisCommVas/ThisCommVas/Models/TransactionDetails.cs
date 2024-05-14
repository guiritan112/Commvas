using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace ThisCommVas.Models
{
    public class CommVasDbContext : DbContext // Rename as needed
    {
        public DbSet<Transaction> Transactions { get; set; }
        // Add other DbSet properties and configurations as needed
    }
}