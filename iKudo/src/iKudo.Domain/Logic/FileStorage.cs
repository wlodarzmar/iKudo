using iKudo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class FileStorage : IFileStorage
    {
        private string webRootPath;
        private string kudoImagesPath;

        public FileStorage(string webRootPath, string kudoImagesPath)
        {
            this.webRootPath = webRootPath;
            this.kudoImagesPath = Path.Combine(webRootPath, kudoImagesPath);

            if (!Directory.Exists(this.kudoImagesPath))
            {
                Directory.CreateDirectory(this.kudoImagesPath);
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

        public void Delete(IEnumerable<string> paths)
        {
            foreach (string path in paths.Where(x => File.Exists(x)))
            {
                File.Delete(path);
            }
        }
    }
}
