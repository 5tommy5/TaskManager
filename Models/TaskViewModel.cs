using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class TaskViewModel
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public IFormFile AttachedFile { get; set; }
        
    }
}
