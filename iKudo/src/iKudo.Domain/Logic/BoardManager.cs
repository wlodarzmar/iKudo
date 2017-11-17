using iKudo.Domain.Criteria;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class BoardManager : IManageBoards, IDisposable
    {
        private const string BoardNotFoundMessage = "Board with specified id does not exist";
        private KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;

        public BoardManager(KudoDbContext dbContext, IProvideTime timeProvider)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
        }

        public Board Add(Board board)
        {
            throw new AlreadyExistException("asdsad");
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            ValidateIfBoardNameExist(board);

            board.CreationDate = timeProvider.Now();
            dbContext.Boards.Add(board);
            dbContext.UserBoards.Add(new UserBoard(board.CreatorId, board.Id));
            dbContext.SaveChanges();

            return board;
        }

        public Board Get(int id)
        {
            return dbContext.Boards.Include(x => x.UserBoards).FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Board> GetAll(BoardSearchCriteria criteria)
        {
            criteria = criteria ?? new BoardSearchCriteria();
            IQueryable<Board> boards = dbContext.Boards;

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
            Board boardToDelete = dbContext.Boards.FirstOrDefault(x => x.Id == id);
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

            board.ModificationDate = timeProvider.Now();

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

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
