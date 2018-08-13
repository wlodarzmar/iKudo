using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IFileStorage
    {
        string Save(string fileName, byte[] content);

        string GenerateFileName();

        string ToRelativePath(string path);

        void Delete(IEnumerable<string> paths);

        void Delete(string path);
    }
}
