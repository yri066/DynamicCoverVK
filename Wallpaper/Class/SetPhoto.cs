using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallpaper.Class
{
    //Используется для получения URL адреса при декодирования JSON, на который будет отправлено изображение
    public class SetPhoto
    {
        public string hash { get; set; }
        public string photo { get; set; }
    }
}
