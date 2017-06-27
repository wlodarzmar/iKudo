namespace iKudo.Domain.Model
{
    public class UserBoard
    {
        public UserBoard() { }

        public UserBoard(string userId, int boardId)
        {
            UserId = userId;
            BoardId = boardId;
        }

        public string UserId { get; set; }

        public int BoardId { get; set; }
    }
}
