using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class KudosManager : IManageKudos
    {
        private readonly KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;
        private readonly IFileStorage fileStorage;

        public KudosManager(KudoDbContext dbContext, IProvideTime timeProvider, IFileStorage fileStorage)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
            this.fileStorage = fileStorage;
        }

        public IEnumerable<KudoType> GetTypes()
        {
            return Enum.GetValues(typeof(KudoType)).Cast<KudoType>();
        }

        public Kudo Add(string userPerformingAction, Kudo kudo)
        {
            ValidateUserPerformingAction(userPerformingAction, kudo.SenderId);
            Board board = dbContext.Boards.AsNoTracking().FirstOrDefault(x => x.Id == kudo.BoardId);
            ValidateSenderAndReceiver(kudo, board);

            if (!string.IsNullOrWhiteSpace(kudo.Image))
            {
                kudo.Image = SaveKudoImage(kudo);
            }

            kudo.CreationDate = timeProvider.Now();
            dbContext.Kudos.Add(kudo);
            AddNotificationAboutKudoAdd(kudo);

            dbContext.SaveChanges();

            return kudo;
        }

        private string SaveKudoImage(Kudo kudo)
        {
            string name = $"{fileStorage.GenerateFileName()}{kudo.ImageExtension}";
            return fileStorage.Save(name, kudo.ImageArray);
        }

        private void ValidateUserPerformingAction(string userPerformingAction, string senderId)
        {
            if (userPerformingAction != senderId)
            {
                throw new UnauthorizedAccessException("You can't add kudo on behalf of other user");
            }
        }

        private void ValidateSenderAndReceiver(Kudo kudo, Board board)
        {
            List<UserBoard> boardUsers = GetUsersOfBoard(kudo.BoardId, kudo.SenderId, kudo.ReceiverId);

            if (board.IsPrivate && !IsMemberOfBoard(kudo.SenderId, boardUsers))
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
            NotificationTypes notificationType = kudo.IsAnonymous ? NotificationTypes.AnonymousKudoAdded : NotificationTypes.KudoAdded;
            Notification notification = new Notification
            {
                SenderId = kudo.SenderId,
                ReceiverId = kudo.ReceiverId,
                CreationDate = timeProvider.Now(),
                Type = notificationType
            };
            notification.BoardId = kudo.BoardId;
            dbContext.Notifications.Add(notification);
        }

        public IEnumerable<Kudo> GetKudos(KudosSearchCriteria criteria)
        {
            IQueryable<Kudo> kudos = dbContext.Kudos
                                              .Include(x => x.Sender).Include(x => x.Receiver)
                                              .Where(x => !x.Board.IsPrivate
                                                        || x.Board.CreatorId == criteria.UserPerformingActionId);

            kudos = FilterByBoard(criteria, kudos);
            kudos = FilterByUser(criteria, kudos);

            foreach (var kudo in kudos)
            {
                HideAnonymousSender(criteria, kudo);
                ChangeToRelativePaths(kudo);
            }

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

        private void ChangeToRelativePaths(Kudo kudo)
        {
            if (!string.IsNullOrWhiteSpace(kudo.Image))
            {
                kudo.Image = fileStorage.ToRelativePath(kudo.Image);
            }
        }

        private void HideAnonymousSender(KudosSearchCriteria criteria, Kudo kudo)
        {
            if (!string.IsNullOrWhiteSpace(criteria.UserPerformingActionId))
            {
                if (kudo.IsAnonymous && kudo.SenderId != criteria.UserPerformingActionId)
                {
                    kudo.SenderId = string.Empty;
                    kudo.Sender = null;
                }
            }
        }
    }
}
