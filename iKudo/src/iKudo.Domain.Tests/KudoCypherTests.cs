using FluentAssertions;
using iKudo.Domain.Configuration;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class KudoCypherTests
    {
        private readonly Mock<IOptions<KudoCypherConfig>> cypherOptionsMock;

        public KudoCypherTests()
        {
            cypherOptionsMock = new Mock<IOptions<KudoCypherConfig>>();
            cypherOptionsMock.Setup(x => x.Value).Returns(new KudoCypherConfig { KudoCypherPrefix = "_!!!---!!!_" });
        }

        [Fact]
        public void Encrypt_AddsPrefixToEncryptedValue_ValueWithPrefix()
        {
            IKudoCypher cypher = new DefaultKudoCypher(cypherOptionsMock.Object);

            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };

            cypher.Encrypt(kudo);

            kudo.Description.Should().StartWith(cypher.Prefix);
        }

        [Fact]
        public void EncryptDecrypt_SameValueInTheEnd()
        {
            IKudoCypher cypher = new DefaultKudoCypher(cypherOptionsMock.Object);

            Kudo kudo = new Kudo { Description = "desc", SenderId = "sender" };
            cypher.Encrypt(kudo);
            cypher.Decrypt(kudo);

            kudo.Description.Should().Be("desc");
        }

        [Fact]
        public void Decrypt_NotEncryptedValue_ReturnsValueWithoutDecryption()
        {
            IKudoCypher cypher = new DefaultKudoCypher(cypherOptionsMock.Object);
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
