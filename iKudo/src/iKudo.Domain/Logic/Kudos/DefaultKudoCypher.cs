using iKudo.Common;
using iKudo.Domain.Configuration;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.Extensions.Options;

namespace iKudo.Domain.Logic
{
    public class DefaultKudoCypher : IKudoCypher
    {
        public DefaultKudoCypher()
        {
            Prefix = string.Empty;
        }

        public DefaultKudoCypher(IOptions<KudoCypherConfig> options)
        {
            Prefix = options.Value.KudoCypherPrefix;
        }

        public string Prefix { get; private set; }

        public void Decrypt(Kudo kudo)
        {
            if (string.IsNullOrWhiteSpace(kudo.Description) || !kudo.Description.StartsWith(Prefix))
            {
                return;
            }

            kudo.Description = kudo.Description?.TrimStart(Prefix.ToCharArray())?.Decrypt(kudo.SenderId);
        }

        public void Encrypt(Kudo kudo)
        {
            if (string.IsNullOrWhiteSpace(kudo.Description))
            {
                return;
            }

            kudo.Description = $"{Prefix}{kudo.Description.Encrypt(kudo.SenderId)}";
        }
    }
}
