using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Extensions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace iKudo.Domain.Logic
{
    public class KudosProvider : IProvideKudos
    {
        private readonly KudoDbContext dbContext;
        private readonly IKudoCypher kudoCypher;
        private readonly IFileStorage fileStorage;

        public KudosProvider(KudoDbContext dbContext, IKudoCypher kudoCypher, IFileStorage fileStorage)
        {
            this.dbContext = dbContext;
            this.kudoCypher = kudoCypher;
            this.fileStorage = fileStorage;
        }

        public IEnumerable<Kudo> GetKudos(KudosSearchCriteria searchCriteria)
        {
            return GetKudos(searchCriteria, null);
        }

        public IEnumerable<Kudo> GetKudos(KudosSearchCriteria searchCriteria, SortCriteria sortCriteria)
        {
            IQueryable<Kudo> kudos = dbContext.Kudos
                                              .Include(x => x.Sender)
                                              .Include(x => x.Receiver)
                                              .Include(x => x.Board)
                                              .Where(x => !x.Board.IsPrivate
                                                        || x.Board.CreatorId == searchCriteria.UserPerformingActionId
                                                        || (x.Board.IsPrivate &&
                                                            x.Board.UserBoards.Select(ub => ub.UserId).Contains(searchCriteria.UserPerformingActionId) &&
                                                            x.Status == KudoStatus.Accepted));

            kudos = FilterByBoard(searchCriteria, kudos);
            kudos = FilterByUser(searchCriteria, kudos);
            kudos = FilterByStatus(searchCriteria, kudos);

            foreach (var kudo in kudos)
            {
                kudo.IsApprovalEnabled = searchCriteria.UserPerformingActionId == kudo.Board.CreatorId && kudo.Status == KudoStatus.New;
                kudoCypher.Decrypt(kudo);
                HideAnonymousSender(searchCriteria, kudo);
                ChangeToRelativePaths(kudo);
            }

            if (sortCriteria != null && !string.IsNullOrWhiteSpace(sortCriteria?.Criteria))
            {
                kudos = kudos.OrderBy(sortCriteria.Criteria);
            }

            return kudos.ToList();
        }

        private IQueryable<Kudo> FilterByStatus(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            kudos = kudos.WhereIf(x => criteria.Statuses.Contains(x.Status), criteria.Statuses.Count() != 0);

            kudos = kudos.Where(x => ((x.SenderId == criteria.UserPerformingActionId ||
                                       x.ReceiverId == criteria.UserPerformingActionId ||
                                       x.Board.CreatorId == criteria.UserPerformingActionId) &&
                                        x.Status != KudoStatus.Accepted)
                                     || x.Status == KudoStatus.Accepted);

            return kudos;
        }

        private static IQueryable<Kudo> FilterByBoard(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            return kudos.WhereIf(x => x.BoardId == criteria.BoardId, criteria.BoardId.HasValue);
        }

        private static IQueryable<Kudo> FilterByUser(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            if (!string.IsNullOrWhiteSpace(criteria.User))
            {
                switch (criteria.UserSearchType)
                {
                    case UserSearchTypes.Both:
                        kudos = kudos.Where(x => x.ReceiverId == criteria.User || x.SenderId == criteria.User);
                        break;
                    case UserSearchTypes.SenderOnly:
                        kudos = kudos.Where(x => x.SenderId == criteria.User);
                        break;
                    case UserSearchTypes.ReceiverOnly:
                        kudos = kudos.Where(x => x.ReceiverId == criteria.User);
                        break;
                    default:
                        break;
                }
            }

            return kudos;
        }

        private void ChangeToRelativePaths(Kudo kudo)
        {
            if (!string.IsNullOrWhiteSpace(kudo.Image))
            {
                kudo.Image = fileStorage.ToRelativePath(kudo.Image);
            }
        }

        private void HideAnonymousSender(KudosSearchCriteria criteria, Kudo kudo)
        {
            if (kudo.IsAnonymous && !IsSenderSameAsCurrentUser(criteria, kudo))
            {
                kudo.SenderId = string.Empty;
                kudo.Sender = null;
            }
        }

        private bool IsSenderSameAsCurrentUser(KudosSearchCriteria criteria, Kudo kudo)
        {
            return !string.IsNullOrWhiteSpace(criteria.UserPerformingActionId) && kudo.SenderId == criteria.UserPerformingActionId;
        }
    }
}
