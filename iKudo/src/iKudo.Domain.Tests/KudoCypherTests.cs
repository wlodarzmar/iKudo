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

        [Fact]
        public void Decrypt_NotEncryptedValue_ReturnsValueWithoutDecryption()
        {
            IKudoCypher cypher = new DefaultKudoCypher("_!!-!!_");
            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };

            cypher.Decrypt(kudo);

            kudo.Description.Should().Be("desc");
        }

        [Fact]
        public void Decrypt_EncryptedValueWithoutPrefix_ReturnsDecryptedValue()
        {
            IKudoCypher cypher = new DefaultKudoCypher();
            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };

            cypher.Encrypt(kudo);
            cypher.Decrypt(kudo);

            kudo.Description.Should().Be("desc");
        }

        [Fact]
        public void Encrypt_WithoutPrefix_EncryptsValue()
        {
            IKudoCypher cypher = new DefaultKudoCypher();
            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };

            cypher.Encrypt(kudo);

            Assert.NotEqual("desc", kudo.Description);
        }

        [Fact]
        public void Encrypt_WithNullValue_DoesNothing()
        {
            IKudoCypher cypher = new DefaultKudoCypher();
            Kudo kudo = new Kudo { SenderId = "sender" };

            cypher.Encrypt(kudo);

            kudo.Description.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithNullValue_DoesNothing()
        {
            IKudoCypher cypher = new DefaultKudoCypher();
            Kudo kudo = new Kudo { SenderId = "sender" };

            cypher.Decrypt(kudo);

            kudo.Description.Should().BeNull();
        }
    }
}
