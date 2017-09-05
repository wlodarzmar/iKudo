using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iKudo.Domain.Criteria;

namespace iKudo.Parsers
{
    public class KudoSearchCriteriaParser : IKudoSearchCriteriaParser
    {
        public KudosSearchCriteria Parse(string currentUser, int? boardId, string sender, string receiver, string senderOrReceiver)
        {
            KudosSearchCriteria criteria = new KudosSearchCriteria
            {
                BoardId = boardId,
                UserPerformingActionId = currentUser
            };

            if (!string.IsNullOrWhiteSpace(receiver))
            {
                criteria.User = receiver;
                criteria.UserSearchType = UserSearchTypes.ReceiverOnly;
            }
            else if (!string.IsNullOrWhiteSpace(sender))
            {
                criteria.User = sender;
                criteria.UserSearchType = UserSearchTypes.SenderOnly;
            }
            else if (!string.IsNullOrWhiteSpace(senderOrReceiver))
            {
                criteria.User = senderOrReceiver;
            }

            return criteria;
        }
    }
}
