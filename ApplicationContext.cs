using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ApplicationContext : DbContext
    {
        // место для хранения задач
        public DbSet<Models.Task> Tasks { get; set; }
        // место для хранения емейлов
        public DbSet<Models.Email> Emails { get; set; }
        // емейл и пароль почты с которой будут отправлятся оповещения
        public string SendMail = "#";
        public string SendPassword = "#";
        // Конструктор контекста
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            // Функция создания бд
            Database.EnsureCreated();
            // Функция удаления бд
            //Database.EnsureDeleted();
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

    }
}
