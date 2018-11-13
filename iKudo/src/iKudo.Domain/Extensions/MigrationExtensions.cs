using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.Reflection;

namespace iKudo.Domain.Extensions
{
    static class MigrationExtensions
    {
        public static string ReadSqlScript(this Migration migration, string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
