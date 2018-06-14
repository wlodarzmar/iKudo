using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

namespace iKudo.Domain.Logic
{
    public class BoardInvitationEmailGenerator : IGenerateBoardInvitationEmail
    {
        public BoardInvitationEmailGenerator(string fromEmail, string invitationAcceptUrl)
        {
            FromEmail = fromEmail;
            BoardInvitationAcceptUrl = invitationAcceptUrl;
        }

        public string FromEmail { get; protected set; }
        public string BoardInvitationAcceptUrl { get; protected set; }

        public BoardInvitation Invitation { get; set; }

        public string GenerateContent()
        {
            string content = $"{Invitation.Creator?.Name} have invited you to kudo board." +
                $"Please click following link to accept invitation: {GenerateLink()}";

            return content;
        }

        private object GenerateLink()
        {
            return $"{BoardInvitationAcceptUrl}?code={Invitation.Code}";
        }

        public string GenerateSubject()
        {
            string subject = $"Invitation to kudo board {Invitation.Board?.Name}.";
            return subject;
        }
    }
}
