using FluentAssertions;
using iKudo.Common;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void CanEncryptAndDecryptString_EncryptedString()
        {
            string text = "asd";
            string encryptedText = text.Encrypt("key");
            string decryptedText = encryptedText.Decrypt("key");

            encryptedText.Should().NotBeNull();
            Assert.NotEqual(text, encryptedText);
            decryptedText.Should().Be(text);
        }
    }
}
