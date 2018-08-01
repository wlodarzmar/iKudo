using iKudo.Domain.Configuration;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.Extensions.Options;

namespace iKudo.Domain.Logic
{
    public class BoardInvitationEmailGenerator : IGenerateBoardInvitationEmail
    {
        public BoardInvitationEmailGenerator(IOptions<BoardsConfig> boardsOptions)
        {
            FromEmail = boardsOptions.Value.InvitationFromEmail;
            BoardInvitationAcceptUrlFormat = boardsOptions.Value.InvitationAcceptUrlFormat;
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
