using iKudo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;

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

        public IEnumerable<Kudo> GetKudos(string userPerformingAction, int? boardId)
        {
            IQueryable<Kudo> kudos = dbContext.Kudos;

            if (boardId.HasValue)
            {
                kudos = kudos.Where(x => x.BoardId == boardId.Value);
            }

            if (!string.IsNullOrWhiteSpace(userPerformingAction))
            {
                HideAnonymousSenders(userPerformingAction, kudos);
            }

            return kudos.ToList();
        }

        private void HideAnonymousSenders(string userPerformingAction, IQueryable<Kudo> kudos)
        {
            foreach (var kudo in kudos)
            {
                if (kudo.IsAnonymous && kudo.SenderId != userPerformingAction)
                {
                    kudo.SenderId = string.Empty;
                }
            }
        }
    }
}
