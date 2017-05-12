using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using iKudo.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace iKudo.Domain.Logic
{
    public class BoardManager : IBoardManager, IDisposable
    {
        private const string BoardNotFoundMessage = "Board with specified id does not exist";
        private KudoDbContext dbContext;
        private readonly ITimeProvider timeProvider;

        public BoardManager(KudoDbContext dbContext, ITimeProvider timeProvider)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
        }

        public Board Add(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            ValidateIfBoardNameExist(board);

            board.CreationDate = timeProvider.Now();
            dbContext.Boards.Add(board);
            dbContext.SaveChanges();

            return board;
        }

        public Board Get(int id)
        {
            return dbContext.Boards.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Board> GetAll()
        {
            return dbContext.Boards.ToList();
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

        public JoinRequest Join(int boardId, string candidateId)
        {
            Board board = dbContext.Boards.Include(x => x.JoinRequests).FirstOrDefault(x => x.Id == boardId);

            if (board == null)
            {
                throw new NotFoundException(BoardNotFoundMessage);
            }

            if (candidateId == board.CreatorId)
            {
                throw new InvalidOperationException("You cannot join to your own board");
            }

            if (board.JoinRequests.Any(x => !x.IsAccepted && x.CandidateId == candidateId))
            {
                throw new InvalidOperationException("There is not accepted request already");
            }

            if (board.UserBoards.Any(x => x.BoardId == boardId && x.UserId == candidateId))
            {
                throw new InvalidOperationException("User is a member of this board already");
            }

            JoinRequest joinRequest = new JoinRequest
            {
                BoardId = boardId,
                Board = board,
                CandidateId = candidateId,
                CreationDate = timeProvider.Now(),
            };

            board.JoinRequests.Add(joinRequest);
            dbContext.SaveChanges();

            return joinRequest;
        }
    }
}
