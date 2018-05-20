using iKudo.Domain.Enums;
using iKudo.Domain.Model;

namespace iKudo.Domain.Tests
{
    static class BoardExtensions
    {
        public static Board WithPublicity(this Board board, bool isPublic)
        {
            board.IsPrivate = !isPublic;

            return board;
        }

        public static Board WithAcceptance(this Board board, AcceptanceType acceptanceType)
        {
            board.AcceptanceType = acceptanceType;

            return board;
        }
    }
}
