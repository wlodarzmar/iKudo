using iKudo.Domain.Criteria;

namespace iKudo.Parsers
{
    public class UserSearchCriteriaParser : IUserSearchCriteriaParser
    {
        public UserSearchCriteria Parse(int boardId, string except)
        {
            return new UserSearchCriteria
            {
                BoardId = boardId,
                Exclude = except?.Split(',') ?? new string[] { }
            };
        }
    }
}
