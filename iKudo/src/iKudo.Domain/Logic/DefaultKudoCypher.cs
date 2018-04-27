using iKudo.Common;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

namespace iKudo.Domain.Logic
{
    public class DefaultKudoCypher : IKudoCypher
    {
        public DefaultKudoCypher()
        {
            Prefix = string.Empty;
        }

        public DefaultKudoCypher(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; private set; }

        public void Decrypt(Kudo kudo)
        {
            kudo.Description = kudo.Description?.TrimStart(Prefix.ToCharArray())?.Decrypt(kudo.SenderId);
        }

        public void Encrypt(Kudo kudo)
        {
            kudo.Description = $"{Prefix}{kudo.Description.Encrypt(kudo.SenderId)}";
        }
    }
}
