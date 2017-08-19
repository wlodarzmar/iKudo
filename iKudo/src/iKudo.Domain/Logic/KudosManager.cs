using iKudo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using iKudo.Domain.Criteria;

namespace iKudo.Domain.Logic
{
    public class KudosManager : IManageKudos
    {
        private KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;

        public KudosManager(KudoDbContext dbContext, IProvideTime timeProvider)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
        }

        public IEnumerable<KudoType> GetTypes()
        {
            return Enum.GetValues(typeof(KudoType)).Cast<KudoType>();
        }

        public Kudo Add(string userPerformingAction, Kudo kudo)
        {
            ValidateUserPerformingAction(userPerformingAction, kudo.SenderId);
            ValidateSenderAndReceiver(kudo);

            kudo.CreationDate = timeProvider.Now();
            dbContext.Kudos.Add(kudo);
            AddNotificationAboutKudoAdd(kudo);
            dbContext.SaveChanges();

            return kudo;
        }

        private void ValidateUserPerformingAction(string userPerformingAction, string senderId)
        {
            if (userPerformingAction != senderId)
            {
                throw new UnauthorizedAccessException("You can't add kudo on behalf of other user");
            }
        }

        private void ValidateSenderAndReceiver(Kudo kudo)
        {
            List<UserBoard> boardUsers = GetUsersOfBoard(kudo.BoardId, kudo.SenderId, kudo.ReceiverId);

            if (!IsMemberOfBoard(kudo.SenderId, boardUsers))
            {
                throw new UnauthorizedAccessException("You can't add kudo to board with given id");
            }

            if (!IsMemberOfBoard(kudo.ReceiverId, boardUsers))
            {
                throw new InvalidOperationException("You can't add kudo for given receiver. Receiver is not member of specified board");
            }
        }

        private List<UserBoard> GetUsersOfBoard(int boardId, params string[] users)
        {
            return dbContext.UserBoards
                            .AsNoTracking()
                            .Where(x => x.BoardId == boardId && users.Contains(x.UserId))
                            .ToList();
        }

        private static bool IsMemberOfBoard(string userId, List<UserBoard> boardUsers)
        {
            return boardUsers.Any(x => x.UserId == userId);
        }

        private void AddNotificationAboutKudoAdd(Kudo kudo)
        {
            Notification notification = new Notification(kudo.SenderId, kudo.ReceiverId, timeProvider.Now(), NotificationTypes.KudoAdded);
            notification.BoardId = kudo.BoardId;
            dbContext.Notifications.Add(notification);
        }

        public IEnumerable<Kudo> GetKudos(KudosSearchCriteria criteria)
        {
            IQueryable<Kudo> kudos = dbContext.Kudos;

            kudos = FilterByBoard(criteria, kudos);
            kudos = FilterByUser(criteria, kudos);
            HideAnonymousSenders(criteria, kudos);

            return kudos.ToList();
        }

        private static IQueryable<Kudo> FilterByBoard(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            if (criteria.BoardId.HasValue)
            {
                kudos = kudos.Where(x => x.BoardId == criteria.BoardId.Value);
            }

            return kudos;
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

        private void HideAnonymousSenders(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            if (!string.IsNullOrWhiteSpace(criteria.UserPerformingActionId))
            {
                foreach (var kudo in kudos)
                {
                    if (kudo.IsAnonymous && kudo.SenderId != criteria.UserPerformingActionId)
                    {
                        kudo.SenderId = string.Empty;
                    }
                }
            }
        }
    }
}
