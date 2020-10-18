using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Models.Email> Emails { get; set; }
        public string SendMail = "toxaman228@gmail.com";
        public string SendPassword = "HJKnjy20012017";
        //public string Email = "tomchuk.anton1@gmail.com";

        //public DbSet<Models.File> Files { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
            //Database.EnsureDeleted();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

    }
}
