using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using Moq;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerBaseTest : BaseTest
    {
        public KudosManagerBaseTest()
        {
            FileStorageMock = new Mock<ISaveFiles>();
            Manager = new KudosManager(DbContext, TimeProviderMock.Object, FileStorageMock.Object);
        }

        public IManageKudos Manager { get; set; }

        public Mock<ISaveFiles> FileStorageMock { get; set; }
    }
}
