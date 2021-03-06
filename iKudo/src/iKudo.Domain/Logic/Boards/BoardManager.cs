﻿using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace iKudo.Domain.Logic
{
    public class BoardManager : IManageBoards
    {
        private const string BoardNotFoundMessage = "Board with specified id does not exist";
        private readonly KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;
        private readonly IFileStorage fileStorage;
        private readonly ISendEmails emailSender;
        private readonly IGenerateBoardInvitationEmail boardInvitationGenerator;

        public BoardManager(
            KudoDbContext dbContext,
            IProvideTime timeProvider,
            IFileStorage fileStorage,
            ISendEmails emailSender,
            IGenerateBoardInvitationEmail boardInvitationGenerator)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
            this.fileStorage = fileStorage;
            this.emailSender = emailSender;
            this.boardInvitationGenerator = boardInvitationGenerator;
        }

        public Board Add(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            ValidateIfBoardNameExist(board);
            ValidateBoard(board);

            board.Add(timeProvider.Now());

            dbContext.Boards.Add(board);
            dbContext.SaveChanges();

            return board;
        }

        public void Delete(string userId, int id)
        {
            Board boardToDelete = dbContext.Boards.Include(x => x.Kudos).FirstOrDefault(x => x.Id == id);
            if (boardToDelete == null)
            {
                throw new NotFoundException("Obiekt o podanym identyfikatorze nie istnieje");
            }

            if (!string.Equals(boardToDelete.CreatorId, userId))
            {
                throw new UnauthorizedAccessException("Nie masz dostępu do tego obiektu");
            }

            dbContext.Boards.Remove(boardToDelete);
            dbContext.SaveChanges();

            IEnumerable<string> imagesToDelete = boardToDelete.Kudos
                                                              .Where(x => x.Image != null)
                                                              .Select(x => x.Image);
            fileStorage.Delete(imagesToDelete);
        }

        public void Update(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (dbContext.Boards.AsNoTracking().FirstOrDefault(x => x.Id == board.Id)?.CreatorId != board.CreatorId)
            {
                throw new UnauthorizedAccessException("Nie masz dostępu do tego obiektu");
            }

            ValidateIfBoardNameExist(board);
            ValidateIfBoardExist(board);
            ValidateBoard(board);

            board.Update(timeProvider.Now());

            dbContext.Update(board);
            dbContext.SaveChanges();
        }

        private void ValidateIfBoardExist(Board board)
        {
            if (!dbContext.Boards.AsNoTracking().Any(x => x.Id == board.Id))
            {
                throw new NotFoundException(BoardNotFoundMessage);
            }
        }

        private void ValidateIfBoardNameExist(Board board)
        {
            if (dbContext.Boards.AsNoTracking().Any(x => (x.Id != board.Id || board.Id == 0) && x.Name == board.Name))
            {
                throw new AlreadyExistException($"Board '{board.Name}' already exists");
            }
        }

        private void ValidateBoard(Board board)
        {
            if (board.IsPrivate && board.AcceptanceType == AcceptanceType.FromExternalUsersOnly)
            {
                throw new ValidationException($"Cannot add private board with acceptance type '{AcceptanceType.FromExternalUsersOnly}'");
            }
        }

        public async Task<List<KeyValuePair<string, HttpStatusCode>>> Invite(string userId, int boardId, string[] emails)
        {
            if (!IsUserOwnerOfBoard(userId, boardId))
            {
                throw new UnauthorizedAccessException("Cannot send invites. You don't have access to this board");
            }

            if (IsUserOnInvitationList(userId, emails))
            {
                throw new KudoException("You cannot invite yourself");
            }

            var invitations = AddInvitations(userId, boardId, emails);

            var sendResults = await SendEmailInvitations(invitations);

            dbContext.SaveChanges();

            return sendResults;
        }

        private bool IsUserOnInvitationList(string userId, string[] emails)
        {
            return dbContext.Users.Any(x => emails.Contains(x.Email) && x.Id == userId);
        }

        private async Task<List<KeyValuePair<string, HttpStatusCode>>> SendEmailInvitations(IEnumerable<BoardInvitation> invitations)
        {
            var mailsToSend = new List<MailMessage>();
            foreach (var invitation in invitations)
            {
                boardInvitationGenerator.Invitation = invitation;
                string subject = boardInvitationGenerator.GenerateSubject();
                string content = boardInvitationGenerator.GenerateContent();

                MailMessage mail = new MailMessage(boardInvitationGenerator.FromEmail, invitation.Email, subject, content);
                mailsToSend.Add(mail);
            }

            return await emailSender.SendAsync(mailsToSend);
        }

        private IEnumerable<BoardInvitation> AddInvitations(string user, int boardId, string[] emails)
        {
            var existingBoardInvitations = dbContext.BoardInvitations
                                                    .Where(x => x.BoardId == boardId && x.IsActive && emails.Contains(x.Email));

            var addedInvitations = new List<BoardInvitation>();
            foreach (var email in emails.Distinct())
            {
                var existingInvitation = existingBoardInvitations.FirstOrDefault(x => x.Email == email);
                if (InvitationExistForThisEmailAndBoard(email, existingBoardInvitations))
                {
                    ArchiveInvitation(existingInvitation);
                }

                addedInvitations.Add(AddInvitation(user, boardId, email));
            }

            return addedInvitations;
        }

        private BoardInvitation AddInvitation(string user, int boardId, string email)
        {
            var invitation = new BoardInvitation
            {
                Email = email,
                BoardId = boardId,
                Code = Guid.NewGuid(),
                CreationDate = timeProvider.Now(),
                CreatorId = user,
                IsActive = true,
            };

            dbContext.BoardInvitations.Add(invitation);
            dbContext.Entry(invitation).Reference(x => x.Creator).Load();
            dbContext.Entry(invitation).Reference(x => x.Board).Load();

            return invitation;
        }

        private void ArchiveInvitation(BoardInvitation existingInvitation)
        {
            existingInvitation.IsActive = false;
            dbContext.BoardInvitations.Update(existingInvitation);
        }

        private bool InvitationExistForThisEmailAndBoard(string email, IQueryable<BoardInvitation> existingInvitations)
        {
            return existingInvitations.Any(x => x.Email == email);
        }

        private bool IsUserOwnerOfBoard(string user, int boardId)
        {
            return dbContext.Boards.Any(x => x.Id == boardId && x.CreatorId == user);
        }

        public void AcceptInvitation(string userId, int boardId, string code)
        {
            BoardInvitation invitation = dbContext.BoardInvitations.FirstOrDefault(x => x.BoardId == boardId && x.Code.ToString() == code);
            if (invitation == null)
            {
                throw new InvalidOperationException("Cannot find a board invitation for given arguments");
            }

            if (!invitation.IsActive)
            {
                throw new KudoException("Board invitation is not active");
            }

            Board board = dbContext.Boards.FirstOrDefault(x => x.Id == boardId);

            var userBoard = new UserBoard(userId, boardId);
            dbContext.UserBoards.Add(userBoard);

            var invitationAcceptedNotification = new Notification
            {
                BoardId = boardId,
                CreationDate = timeProvider.Now(),
                ReceiverId = board.CreatorId,
                SenderId = userId,
                Type = NotificationTypes.BoardInvitationAccepted
            };
            dbContext.Notifications.Add(invitationAcceptedNotification);
            invitation.IsActive = false;
            dbContext.SaveChanges();
        }
    }
}