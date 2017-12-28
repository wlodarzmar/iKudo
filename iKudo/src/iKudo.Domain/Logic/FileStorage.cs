using iKudo.Domain.Interfaces;
using System;
using System.IO;

namespace iKudo.Domain.Logic
{
    public class FileStorage : ISaveFiles
    {
        private string webRootPath;

        public FileStorage(string kudoImagesPath)
        {
            webRootPath = kudoImagesPath;

            if (!Directory.Exists(kudoImagesPath))
            {
                Directory.CreateDirectory(kudoImagesPath);
            }
        }

        public string GenerateFileName()
        {
            return Guid.NewGuid().ToString();
        }

        public string Save(string fileName, byte[] content)
        {
            string path = Path.Combine(webRootPath, fileName);
            File.WriteAllBytes(path, content);

            return path;
        }
    }
}
