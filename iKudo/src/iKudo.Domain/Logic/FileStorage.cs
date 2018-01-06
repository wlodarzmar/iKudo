using iKudo.Domain.Interfaces;
using System;
using System.IO;

namespace iKudo.Domain.Logic
{
    public class FileStorage : ISaveFiles
    {
        private string webRootPath;
        private string kudoImagesPath;

        public FileStorage(string webRootPath, string kudoImagesPath)
        {
            this.webRootPath = webRootPath;
            this.kudoImagesPath = Path.Combine(webRootPath, kudoImagesPath);

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
            string path = Path.Combine(kudoImagesPath, fileName);
            File.WriteAllBytes(path, content);

            return path;
        }

        public string ToRelativePath(string path)
        {
            return path.Replace(webRootPath, "");
        }
    }
}
