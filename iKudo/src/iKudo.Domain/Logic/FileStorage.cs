using iKudo.Domain.Configuration;
using iKudo.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class FileStorage : IFileStorage
    {
        private readonly string webRootPath;
        private readonly string kudoImagesPath;

        public FileStorage(IEnvironment environment, IOptions<PathsConfig> options)
        {
            this.webRootPath = environment.RootPath;
            this.kudoImagesPath = Path.Combine(webRootPath, options.Value.KudoImages);

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

        public void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
