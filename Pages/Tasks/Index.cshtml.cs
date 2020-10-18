using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Pages.Tasks
{
    public class TasksModel : PageModel
    {
        private readonly WebApplication1.ApplicationContext _context;
        public List<Models.Task> DisplayedTasks { get; set; }
        public TasksModel(WebApplication1.ApplicationContext context)
        {
            //context.Tasks.Add(new Models.Task { Name = "Buy a cat", Description = "Call him a SexyBabe", Deadline = Convert.ToDateTime("02/02/2022") });
            //context.SaveChanges();
            _context = context;


        }

        public void OnGet()
        {
            DisplayedTasks = _context.Tasks.ToList();
        }


    }
}
