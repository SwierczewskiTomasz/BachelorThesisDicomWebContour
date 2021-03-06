using System;
using DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Drawing;

namespace Logic
{
    public static class OrthancConnection
    {
        public static string orthancURL = "http://localhost:1337/localhost:8042/instances/";

        public static WebClient client = new WebClient();

        public static Bitmap GetBitmapByInstanceId(string instanceId)
        {
            string url = orthancURL + instanceId + "/preview";
            string tagUrl = orthancURL + instanceId + "/tags";

            Stream stream = client.OpenRead(url);
            Bitmap bitmap = (Bitmap)Image.FromStream(stream);
            stream.Close();
            
            return bitmap;
        }
    }
}
