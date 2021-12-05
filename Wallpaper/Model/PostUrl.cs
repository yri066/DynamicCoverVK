using System;
using System.Collections.Generic;
using System.Text;

namespace Wallpaper.Model
{
    //Используется для получения URL адреса при декодирования JSON, на который будет отправлено изображение 
    public class PostUpload
    {
        public string upload_url { get; set; }
    }

    internal class PostUrl
    {
        public PostUpload response { get; set; }
    }
}
