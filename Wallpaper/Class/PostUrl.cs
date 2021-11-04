using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallpaper.Class
{
    //Используется для получения URL адреса при декодирования JSON,  который будет отправлено изображение 
    public class PostUpload
    {
        public string upload_url { get; set; }
    }

    public class PostUrl
    {
        public PostUpload response { get; set; }
    }
}
