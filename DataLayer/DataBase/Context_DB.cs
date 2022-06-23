using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DataBase
{
    public class Context_DB:DbContext
    {
        public Context_DB(DbContextOptions<Context_DB>Option)
            :base(Option) {     }

        // DbSet

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Token> Tokens { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = 1,
                Email = "ali.moosaei.big@gmail.com",
                FullName = "AliMoosaei",
                UserName = "AliMoosaei",
                HashPassword = "xXUCWL1n3GsjBs+TV4AS6kFm8xnugr8bGPjINMcu4mw=",
                IsActive = true,
                IsAdmin = true,
                IsConfirmEmail = true,
                CounterRequest = 0
            });
            base.OnModelCreating(modelBuilder);
        }
        
    }
}
