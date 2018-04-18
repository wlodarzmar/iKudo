
using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IKudoCypher
    {
        void Encrypt(Kudo kudo);

        void Decrypt(Kudo kudo);
    }
}
