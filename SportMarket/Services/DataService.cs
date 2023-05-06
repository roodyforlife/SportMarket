using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SportMarket.Services
{
    public class DataService : IDataService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public DataService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public byte[] ImageToByte(IFormFile photo)
        {
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(photo.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)photo.Length);
            }
            return imageData;
        }

        public byte[] ImageToByte(string name)
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "img", name);
            FileStream fileStream = File.OpenRead(path);
            BinaryReader reader = new BinaryReader(fileStream);
            return reader.ReadBytes((int)fileStream.Length);
        }
    }
}
