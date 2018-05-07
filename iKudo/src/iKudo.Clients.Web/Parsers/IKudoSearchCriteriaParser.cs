using iKudo.Domain.Criteria;

namespace iKudo.Parsers
{
    public interface IKudoSearchCriteriaParser
    {
        KudosSearchCriteria Parse(string currentUser, int? boardId, string sender, string receiver, string senderOrReceiver, string status);
    }
}
