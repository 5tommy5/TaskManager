using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Pages.Tasks
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class CreateModel : PageModel
    {
        public WebApplication1.ApplicationContext _context;
        [BindProperty]
        public Models.Task TaskCreate { get; set; }
        public CreateModel(WebApplication1.ApplicationContext db)
        {
            _context = db;
        }
        public void OnGet()
        {
        }
        /*public async Task<IActionResult> OnPostAsync(string Name, string Description, DateTime Date)
        {
            TaskCreate.Name = Name;
            TaskCreate.Description = Description;
            TaskCreate.Deadline = Date;


            _context.Tasks.Add(TaskCreate);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");

        }*/
    }
}

