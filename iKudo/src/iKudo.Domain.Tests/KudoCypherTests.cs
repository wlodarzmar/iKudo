using FluentAssertions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class KudoCypherTests
    {
        [Fact]
        public void Encrypt_AddsPrefixToEncryptedValue_ValueWithPrefix()
        {
            IKudoCypher cypher = new DefaultKudoCypher("_!!!---!!!_");

            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };

            cypher.Encrypt(kudo);

            kudo.Description.Should().StartWith(cypher.Prefix);
        }

        [Fact]
        public void EncryptDecrypt_SameValueInTheEnd()
        {
            IKudoCypher cypher = new DefaultKudoCypher("_!!-!!_");

            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };
            cypher.Encrypt(kudo);
            cypher.Decrypt(kudo);

            kudo.Description.Should().Be("desc");
        }
    }
}
