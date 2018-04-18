using iKudo.Common;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

namespace iKudo.Domain.Logic
{
    public class DefaultKudoCypher : IKudoCypher
    {
        public void Decrypt(Kudo kudo)
        {
            kudo.Description = kudo.Description.Decrypt(kudo.SenderId);
        }

        public void Encrypt(Kudo kudo)
        {
            kudo.Description = kudo.Description.Encrypt(kudo.SenderId);
        }
    }
}
