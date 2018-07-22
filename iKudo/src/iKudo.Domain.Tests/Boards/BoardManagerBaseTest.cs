using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using Moq;

namespace iKudo.Domain.Tests
{
    public class BoardManagerBaseTest : BaseTest
    {
        public BoardManagerBaseTest()
        {
            FileStorageMock = new Mock<IFileStorage>();
            EmailSenderMock = new Mock<ISendEmails>();
            BoardInvitationEmailGenerator = new Mock<IGenerateBoardInvitationEmail>();
            BoardInvitationEmailGenerator.Setup(x => x.FromEmail).Returns("from@Email.com");
            BoardInvitationEmailGenerator.Setup(x => x.GenerateContent()).Returns("content");
            BoardInvitationEmailGenerator.Setup(x => x.GenerateSubject()).Returns("subject");
            Manager = new BoardManager(DbContext, TimeProviderMock.Object, FileStorageMock.Object, EmailSenderMock.Object, BoardInvitationEmailGenerator.Object);
        }

        public IManageBoards Manager { get; set; }

        public Mock<IFileStorage> FileStorageMock { get; set; }

        public Mock<ISendEmails> EmailSenderMock { get; set; }

        public Mock<IGenerateBoardInvitationEmail> BoardInvitationEmailGenerator { get; }
    }
}
