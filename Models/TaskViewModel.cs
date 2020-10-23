using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    // ViewModel для задания
    // с ее помощью мы сможем забирать всю нужную информацию о задаче с пользовательского интерфейса
    // Файл хранится в интерфейсе IFormFile
    // с этого интерфейса мы сможем доставать имя файла, его тип, а также размер файла
    public class TaskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public IFormFile AttachedFile { get; set; }        
    }
}
