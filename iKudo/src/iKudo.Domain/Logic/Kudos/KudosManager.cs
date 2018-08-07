using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace iKudo.Domain.Logic
{
    public class KudosManager : IManageKudos
    {
        private readonly KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;
        private readonly IFileStorage fileStorage;
        private readonly IKudoCypher kudoCypher;

        public KudosManager(KudoDbContext dbContext, IProvideTime timeProvider, IFileStorage fileStorage, IKudoCypher kudoCypher)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
            this.fileStorage = fileStorage;
            this.kudoCypher = kudoCypher;
        }

        public IEnumerable<KudoType> GetTypes()
        {
            return Enum.GetValues(typeof(KudoType)).Cast<KudoType>();
        }

        public Kudo Add(string userPerformingActionId, Kudo kudo)
        {
            ValidateUserPerformingAction(userPerformingActionId, kudo.SenderId);
            Board board = dbContext.Boards.Include(x => x.UserBoards).AsNoTracking().FirstOrDefault(x => x.Id == kudo.BoardId);
            ValidateSenderAndReceiver(kudo, board);

            if (!string.IsNullOrWhiteSpace(kudo.Image))
            {
                kudo.Image = SaveKudoImage(kudo);
            }

            if (ShoulKudoBeAccepted(kudo, board))
            {
                kudo.Status = KudoStatus.New;
                AddNotificationAboutKudoToAccept(kudo, board.CreatorId);
            }
            else
            {
                kudo.Status = KudoStatus.Accepted;
            }

            kudo.CreationDate = timeProvider.Now();
            kudoCypher.Encrypt(kudo);
            dbContext.Kudos.Add(kudo);
            AddNotificationAboutKudoAdd(kudo);

            dbContext.SaveChanges();

            return kudo;
        }

        private bool ShoulKudoBeAccepted(Kudo kudo, Board board)
        {
            return board.AcceptanceType == AcceptanceType.All ||
                   (IsExternalUser(kudo.SenderId, board) && board.AcceptanceType == AcceptanceType.FromExternalUsersOnly);
        }

        private bool IsExternalUser(string userId, Board board)
        {
            return !board.UserBoards.Any(x => x.UserId == userId);
        }

        private void AddNotificationAboutKudoToAccept(Kudo kudo, string receiver)
        {
            Notification notification = new Notification
            {
                BoardId = kudo.BoardId,
                CreationDate = timeProvider.Now(),
                ReceiverId = receiver,
                SenderId = kudo.SenderId,
                Type = NotificationTypes.NewKudoToAccept
            };

            dbContext.Notifications.Add(notification);
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
            if (kudo.Status == KudoStatus.New)
            {
                return;
            }

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

        public Kudo Accept(string userPerformingActionId, int kudoId)
        {
            Kudo kudo = dbContext.Kudos.Include(x => x.Board).FirstOrDefault(x => x.Id == kudoId);

            ValidateBoardCreator(userPerformingActionId, kudo);

            kudo.Accept();

            dbContext.Update(kudo);

            AddNotificationAboutKudoAcceptation(kudo, userPerformingActionId);
            AddNotificationAboutKudoAdd(kudo);

            dbContext.SaveChanges();

            return kudo;
        }

        private void AddNotificationAboutKudoAcceptation(Kudo kudo, string userId)
        {
            var kudoAcceptedNotification = new Notification
            {
                BoardId = kudo.BoardId,
                CreationDate = timeProvider.Now(),
                ReceiverId = kudo.SenderId,
                Type = NotificationTypes.KudoAccepted,
                SenderId = userId
            };
            dbContext.Notifications.Add(kudoAcceptedNotification);
        }

        public Kudo Reject(string userPerformingActionId, int kudoId)
        {
            Kudo kudo = dbContext.Kudos.Include(x => x.Board).FirstOrDefault(x => x.Id == kudoId);

            ValidateBoardCreator(userPerformingActionId, kudo);

            kudo.Reject();

            dbContext.Update(kudo);
            AddNotificationAboutKudoRejection(kudo, userPerformingActionId);
            dbContext.SaveChanges();

            return kudo;
        }

        private static void ValidateBoardCreator(string userPerformingActionId, Kudo kudo)
        {
            if (kudo.Board.CreatorId != userPerformingActionId)
            {
                throw new InvalidOperationException("You cannot accept/reject kudos in this board");
            }
        }

        private void AddNotificationAboutKudoRejection(Kudo kudo, string userId)
        {
            var notificationKudoRejection = new Notification
            {
                BoardId = kudo.BoardId,
                CreationDate = timeProvider.Now(),
                ReceiverId = kudo.SenderId,
                SenderId = userId,
                Type = NotificationTypes.KudoRejected
            };
            dbContext.Notifications.Add(notificationKudoRejection);
        }
    }
}
