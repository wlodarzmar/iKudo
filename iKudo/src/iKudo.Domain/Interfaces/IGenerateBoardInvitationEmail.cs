using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IGenerateBoardInvitationEmail
    {
        string GenerateSubject();
        string GenerateContent();
        string FromEmail { get; }
        string BoardInvitationAcceptUrl { get; }

        BoardInvitation Invitation { get; set; }
    }
}
