using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Pages.Tasks
{
    public class ShowModel : PageModel
    {
        private readonly WebApplication1.ApplicationContext _context;

        public ShowModel(WebApplication1.ApplicationContext context)
        {
            _context = context;
        }

        public IList<Models.Task> task;

        public async  void OnGetAsync()
        {
            task = await _context.Tasks.ToListAsync();
        }
    }
}
