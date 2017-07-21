using iKudo.Domain.Criteria;

namespace iKudo.Parsers
{
    public interface IUserSearchCriteriaParser
    {
        UserSearchCriteria Parse(int boardId, string except);
    }
}
