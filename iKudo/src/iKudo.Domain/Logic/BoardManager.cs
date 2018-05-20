using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class BoardManager : IManageBoards
    {
        private const string BoardNotFoundMessage = "Board with specified id does not exist";
        private readonly KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;
        private readonly IFileStorage fileStorage;

        public BoardManager(KudoDbContext dbContext, IProvideTime timeProvider, IFileStorage fileStorage)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
            this.fileStorage = fileStorage;
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

        public Board Get(int id)
        {
            return dbContext.Boards.Include(x => x.UserBoards).FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Board> GetAll(string userId, BoardSearchCriteria criteria)
        {
            criteria = criteria ?? new BoardSearchCriteria();
            IQueryable<Board> boards = dbContext.Boards.Where(x => x.IsPublic || x.CreatorId == userId);

            if (!string.IsNullOrWhiteSpace(criteria.CreatorId))
            {
                boards = boards.Where(x => x.CreatorId == criteria.CreatorId);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Member))
            {
                boards = boards.Where(x => x.UserBoards.Select(ub => ub.UserId).Contains(criteria.Member));
            }

            return boards.ToList();
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
    }
}
