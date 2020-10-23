using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    // Модель емейла
    // В ней сохраняется данные об емейле, на который следует отправлять нотификации
    // а также имя пользователя, для того чтобы в теле сообщения нотификации можно было обращаться к юзеру по имени
    public class Email
    {
        public int Id { get; set; }
        public string email { get; set; }
        public string Name { get; set; }
    }
}
