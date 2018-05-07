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

        public Kudo Add(string userPerforminActionId, Kudo kudo)
        {
            ValidateUserPerformingAction(userPerforminActionId, kudo.SenderId);
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

        public IEnumerable<Kudo> GetKudos(KudosSearchCriteria criteria)
        {
            IQueryable<Kudo> kudos = dbContext.Kudos
                                              .Include(x => x.Sender).Include(x => x.Receiver)
                                              .Where(x => !x.Board.IsPrivate
                                                        || x.Board.CreatorId == criteria.UserPerformingActionId);

            kudos = FilterByBoard(criteria, kudos);
            kudos = FilterByUser(criteria, kudos);
            kudos = FilterByStatus(criteria, kudos);

            foreach (var kudo in kudos)
            {
                HideAnonymousSender(criteria, kudo);
                ChangeToRelativePaths(kudo);
                kudoCypher.Decrypt(kudo);
            }

            return kudos.ToList();
        }

        private IQueryable<Kudo> FilterByStatus(KudosSearchCriteria criteria, IQueryable<Kudo> kudos)
        {
            if (criteria.Status != 0)
            {
                kudos = kudos.Where(x => x.Status == criteria.Status);
            }
            else
            {
                kudos = kudos.Where(x => ((x.SenderId == criteria.UserPerformingActionId || x.ReceiverId == criteria.UserPerformingActionId) &&
                                            x.Status != KudoStatus.Accepted)
                                         || x.Status == KudoStatus.Accepted);
            }

            return kudos;
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

        public Kudo Accept(string userPerformingActionId, int kudoId)
        {
            Kudo kudo = dbContext.Kudos.FirstOrDefault(x => x.Id == kudoId);

            ValidateBoardCreator(userPerformingActionId, kudo);

            kudo.Accept();

            dbContext.Update(kudo);
            dbContext.SaveChanges();

            return kudo;
        }

        public Kudo Reject(string userPerformingActionId, int kudoId)
        {
            Kudo kudo = dbContext.Kudos.FirstOrDefault(x => x.Id == kudoId);

            ValidateBoardCreator(userPerformingActionId, kudo);

            kudo.Reject();

            dbContext.Update(kudo);
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
    }
}
