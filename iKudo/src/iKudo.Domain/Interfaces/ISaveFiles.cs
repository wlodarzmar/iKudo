namespace iKudo.Domain.Interfaces
{
    public interface ISaveFiles
    {
        string Save(string fileName, byte[] content);

        string GenerateFileName();

        string ToRelativePath(string path);
    }
}
