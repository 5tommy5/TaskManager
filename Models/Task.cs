using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    // Модель задачи
    // Хранит основую инфу по задаче
    // Здесь хранятся файлы в виде массива байт
    // а также имя файла и его расширени (ContentType)
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] AttachedFile { get; set; }
    }
}
