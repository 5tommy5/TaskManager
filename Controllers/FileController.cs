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

        public IActionResult Index()
        {
            return View();
        }
        public IWebHostEnvironment _appEnviroment { get; set; }
        private readonly WebApplication1.ApplicationContext _context;
        public FileController(IWebHostEnvironment env, ApplicationContext DB)
        {
            _appEnviroment = env;
            _context = DB;
        }
        [HttpGet]
        public FileResult Download(int id)
        {

            string file_path = Path.Combine(_appEnviroment.ContentRootPath, String.Format("Files\\newcsv{0}.csv", id));
            //Проверка ли файл существует, если нет создает файл
            System.IO.File.Create(file_path).Close();

            Models.Task MyTask = _context.Tasks.Find(id);
            FileDTO MyTaskT = new FileDTO { Id = MyTask.Id, Name = MyTask.Name, Description = MyTask.Description, Deadline = MyTask.Deadline };

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
                    csvReader.WriteRecord<FileDTO>(MyTaskT);
                }
            }
            byte[] mas = System.IO.File.ReadAllBytes(file_path);
            return File(mas, "application/csv", $"Task{id}.csv");
            //using (FileStream streamReader = new FileStream(file_path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    return File(streamReader, "application/csv");
            //}
        }
        [HttpGet]
        public FileResult DownloadAllTasks()
        {
            string file_path = Path.Combine(_appEnviroment.ContentRootPath, "Files\\allrecords.csv");
            //Проверка ли файл существует, если нет создает файл
            System.IO.File.Create(file_path).Close();
            List<FileDTO> records = new List<FileDTO>();
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
        [HttpPost]
        public async Task<IActionResult> AddFile(TaskViewModel tvm)
        {
            Models.Task task = new Models.Task { Name = tvm.Name, Description =tvm.Description, Deadline = tvm.Deadline};

            if (tvm.AttachedFile != null)
            {
                byte[] Data = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(tvm.AttachedFile.OpenReadStream()))
                {
                    Data = binaryReader.ReadBytes((int)tvm.AttachedFile.Length);
                }
                // установка массива байтов
                task.FileName = tvm.AttachedFile.FileName;
                task.ContentType = tvm.AttachedFile.ContentType;
                task.AttachedFile = Data;
            }
            _context.Tasks.Add(task);

            _context.SaveChanges();

            EmailService emailService = new EmailService(_context);
            await emailService.SendEmailAsync(_context.Emails.ToList().Last().email, "Task Manager", $"Дорогой/(ая) {_context.Emails.ToList().Last().Name}, Вам добавлено задание в список! \n{task.Name} - {task.Description}\n Дедлайн: {task.Deadline.ToString()}\nЧтобы посмотреть добавленные файлы перейдите в приложение!\nУдачи!");
            

            return RedirectPermanent("~/Tasks/");

        }
        [HttpGet]
        public FileResult DownloadAttachedFile(int id)
        {
            Models.Task MyTask = _context.Tasks.Find(id);

            return File(MyTask.AttachedFile, MyTask.ContentType, MyTask.FileName);
        }
        [HttpPost]
        public async Task<IActionResult> ChooseEmail(string email, string name)
        {
            _context.Emails.Add(new Email { email = email, Name = name });
            await _context.SaveChangesAsync();
            return RedirectPermanent("~/Tasks/");
        }
        /*

        public async Task<IActionResult> SetEmail(string email)
        {
            if (_context.SetMail(email))
            {
                await _context.SaveChangesAsync();
                return RedirectPermanent("~/Tasks/");
            }
            else
                return RedirectPermanent("~/Home");
        }
        [HttpPut]
        public async Task<IActionResult> ResetEmail(string email)
        {
            if (_context.SetMail(email))
            {
                await _context.SaveChangesAsync();
                return RedirectPermanent("~/Tasks/");
            }
            else
                return RedirectPermanent("~/Home");
        }
        */
    }


}

