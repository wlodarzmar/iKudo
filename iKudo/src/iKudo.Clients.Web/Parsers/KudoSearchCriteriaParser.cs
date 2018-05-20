using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using System;
using System.Collections.Generic;

namespace iKudo.Parsers
{
    public class KudoSearchCriteriaParser : IKudoSearchCriteriaParser
    {
        public KudosSearchCriteria Parse(
            string currentUser,
            int? boardId,
            string sender,
            string receiver,
            string senderOrReceiver,
            string status)
        {
            KudosSearchCriteria criteria = new KudosSearchCriteria
            {
                BoardId = boardId,
                UserPerformingActionId = currentUser,
            };

            if (!string.IsNullOrWhiteSpace(sender) && !string.IsNullOrWhiteSpace(receiver) && string.IsNullOrWhiteSpace(senderOrReceiver))
            {
                if (sender == receiver)
                {
                    criteria.User = sender;
                }
                else
                {
                    throw new ArgumentException($"If both sender and receiver are passed, they must be the same");
                }
            }
            else if (!string.IsNullOrWhiteSpace(senderOrReceiver))
            {
                criteria.User = senderOrReceiver;
            }
            else if (!string.IsNullOrWhiteSpace(receiver))
            {
                criteria.User = receiver;
                criteria.UserSearchType = UserSearchTypes.ReceiverOnly;
            }
            else if (!string.IsNullOrWhiteSpace(sender))
            {
                criteria.User = sender;
                criteria.UserSearchType = UserSearchTypes.SenderOnly;
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                List<KudoStatus> result = new List<KudoStatus>();
                var statusesStr = status.Split(',');
                foreach (var statusStr in statusesStr)
                {
                    result.Add(Enum.Parse<KudoStatus>(statusStr.Trim(), true));
                }

                criteria.Statuses = result.ToArray();
            }

            return criteria;
        }
    }
}
