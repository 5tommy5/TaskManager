using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class FileController : Controller
    {
        // контроллер для работы с файлами

        public IActionResult Index()
        {
            return View();
        }
        public IWebHostEnvironment _appEnviroment { get; set; }
        // добавляем контекст данных
        private readonly WebApplication1.ApplicationContext _context;
        // в конструкторе связываем бд с контекстом
        public FileController(IWebHostEnvironment env, ApplicationContext DB)
        {
            _appEnviroment = env;
            _context = DB;
        }
        // Метод GET для получения данных
        // Метод для скачивания файла с данными о конкретной задаче
        // Принимает один аргумент - ид задачи
        // На выход отдает обьект FileResult, который и позволяет нам с браузера скачать файл
        [HttpGet]
        public FileResult Download(int id)
        {
            // Файлы с информацией о задачах мы будем сохранять на сервере
            // для этого мы получем путь для сохранения
            string file_path = Path.Combine(_appEnviroment.ContentRootPath, String.Format("Files\\newcsv{0}.csv", id));
            //Проверка ли файл существует, если нет создает файл
            System.IO.File.Create(file_path).Close();
            // Создает обьект задачи, в который записывает задачу с заданным ид
            Models.Task MyTask = _context.Tasks.Find(id);
            // Создаем обьект FileDTO, в который записываем нужные нам данные
            FileDTO MyTaskT = new FileDTO { Id = MyTask.Id, Name = MyTask.Name, Description = MyTask.Description, Deadline = MyTask.Deadline };

            using (StreamWriter streamReader = new StreamWriter(file_path))
            {
                //streamReader.WriteLine("sep=,");//добавить для корректного открытия в екселе
                // также для корректого открытия в екселе нужно поменять в нем кодировку
                // файлы корректно открываются в других программах, например 
                using (CsvWriter csvReader = new CsvWriter(streamReader, new CultureInfo("")))
                {
                    // указываем разделитель, который будет использоваться в файле
                    csvReader.Configuration.Delimiter = ",";
                    // Записываем заголовки столбцов
                    csvReader.WriteHeader<FileDTO>();
                    csvReader.NextRecord();
                    // записываем данные в csv файл
                    csvReader.WriteRecord<FileDTO>(MyTaskT);
                }
            }
            // считываем в массив байт файл csv
            byte[] mas = System.IO.File.ReadAllBytes(file_path);
            // возвращаем файл
            return File(mas, "application/csv", $"Task{id}.csv");

        }
        // Метод для получения csv с информацией про все задачи 
        [HttpGet]
        public FileResult DownloadAllTasks()
        {
            string file_path = Path.Combine(_appEnviroment.ContentRootPath, "Files\\allrecords.csv");
            //Проверка ли файл существует, если нет создает файл
            System.IO.File.Create(file_path).Close();
            // Создаем список для задач
            List<FileDTO> records = new List<FileDTO>();
            // добавляем все задачи в этот список
            records.AddRange(_context.Tasks.ToList().ConvertAll(x => new FileDTO() { Id = x.Id, Name = x.Name, Description = x.Description, Deadline = x.Deadline }));
            using (StreamWriter streamReader = new StreamWriter(file_path))
            {
                //streamReader.WriteLine("sep=,");//добавить для корректного открытия в екселе
                using (CsvWriter csvReader = new CsvWriter(streamReader, new CultureInfo("")))
                {
                    // указываем разделитель, который будет использоваться в файле
                    csvReader.Configuration.Delimiter = ",";
                    csvReader.WriteHeader<FileDTO>();
                    csvReader.NextRecord();
                    //csvReader.Configuration.RegisterClassMap<MyCsvMap>(); не нужно, корректо выводится и без него
                    // записываем данные в csv файл
                    csvReader.WriteRecords<FileDTO>(records);
                }
            }
            byte[] mas = System.IO.File.ReadAllBytes(file_path);
            return File(mas, "application/csv", $"AllTasks.csv");
        }

        public Models.Task TaskCreate { get; set; }
        // Создание задания
        [HttpPost]
        public async Task<IActionResult> AddFile(TaskViewModel tvm)
        {
            // Получаем обьект TaskViewModel
            // из него забираем данные 
            Models.Task task = new Models.Task { Name = tvm.Name, Description =tvm.Description, Deadline = tvm.Deadline};
            // Проверяем ли добавлялся файл к заданию
            if (tvm.AttachedFile != null)
            {
                // создаем массив байт
                byte[] Data = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(tvm.AttachedFile.OpenReadStream()))
                {
                    Data = binaryReader.ReadBytes((int)tvm.AttachedFile.Length);
                }
                // добавляем имя файла
                task.FileName = tvm.AttachedFile.FileName;
                // добавляем тип файла
                task.ContentType = tvm.AttachedFile.ContentType;
                // добавляем массив байт, который и является файлом
                task.AttachedFile = Data;
            }
            // добавляем в бд данную задачу
            _context.Tasks.Add(task);

            _context.SaveChanges();
            // используем сервис емейлов для отправки нотификации
            EmailService emailService = new EmailService(_context);
            await emailService.SendEmailAsync(_context.Emails.ToList().Last().email, "Task Manager", $"Дорогой/(ая) {_context.Emails.ToList().Last().Name}, Вам добавлено задание в список! \n{task.Name} - {task.Description}\n Дедлайн: {task.Deadline.ToString()}\nЧтобы посмотреть добавленные файлы перейдите в приложение!\nУдачи!");
            
            // редиректим на страницу отображения таска
            return RedirectPermanent("~/Tasks/");

        }
        // метод для загрузки аттаченых файлов
        [HttpGet]
        public FileResult DownloadAttachedFile(int id)
        {
            // получаем ид задания
            // по ид находим конкретный таск
            Models.Task MyTask = _context.Tasks.Find(id);
            // возвращаем на клиент файл
            return File(MyTask.AttachedFile, MyTask.ContentType, MyTask.FileName);
        }
        // метод для добавления емейла
        [HttpPost]
        public async Task<IActionResult> ChooseEmail(string email, string name)
        {
            // добавляет в базу емейл и соответствующее имя
            _context.Emails.Add(new Email { email = email, Name = name });
            await _context.SaveChangesAsync();
            // редирект на страницу отображения тасков
            return RedirectPermanent("~/Tasks/");
        }
    }


}

