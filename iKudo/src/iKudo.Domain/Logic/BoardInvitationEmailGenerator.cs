using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

namespace iKudo.Domain.Logic
{
    public class BoardInvitationEmailGenerator : IGenerateBoardInvitationEmail
    {
        public BoardInvitationEmailGenerator(string fromEmail, string invitationAcceptUrlFormat)
        {
            FromEmail = fromEmail;
            BoardInvitationAcceptUrlFormat = invitationAcceptUrlFormat;
        }

        public string FromEmail { get; protected set; }
        public string BoardInvitationAcceptUrlFormat { get; protected set; }

        public BoardInvitation Invitation { get; set; }

        public string GenerateContent()
        {
            string content = $"{Invitation.Creator?.Name} have invited you to kudo board '<strong>{Invitation.Board?.Name}</strong>'. " +
                $"Please click following link to accept invitation: {GenerateLink()}";

            return content;
        }

        private object GenerateLink()
        {
            return string.Format(BoardInvitationAcceptUrlFormat, Invitation.Code, Invitation.BoardId);
        }

        public string GenerateSubject()
        {
            string subject = $"Invitation to kudo board '{Invitation.Board?.Name}'.";
            return subject;
        }
    }
}
