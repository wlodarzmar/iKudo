using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class JoinManager : IManageJoins, IDisposable
    {
        private const string BoardNotFoundMessage = "Board with specified id does not exist";
        private KudoDbContext dbContext;
        private readonly IProvideTime timeProvider;

        public JoinManager(KudoDbContext dbContext, IProvideTime timeProvider)
        {
            this.dbContext = dbContext;
            this.timeProvider = timeProvider;
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

            if (board.JoinRequests.Any(x => !x.IsAccepted.HasValue && x.CandidateId == candidateId))
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

        public ICollection<JoinRequest> GetJoinRequests(string userId)
        {
            return dbContext.JoinRequests.Where(x => x.CandidateId == userId).ToList();
        }

        public JoinRequest AcceptJoin(string joinRequestId, string userIdPerformingAction)
        {
            JoinRequest joinRequest = dbContext.JoinRequests.Include(x => x.Board).FirstOrDefault(x => x.Id == joinRequestId);
            ValidateJoinRequestBeforeDecision(userIdPerformingAction, joinRequest);

            joinRequest.Accept(userIdPerformingAction, timeProvider.Now());

            return joinRequest;
        }

        private static void ValidateJoinRequestBeforeDecision(string userIdPerformingAction, JoinRequest joinRequest)
        {
            if (joinRequest == null)
            {
                throw new NotFoundException("JoinRequest with given id does not exist");
            }
            if (joinRequest.IsAccepted.HasValue)
            {
                if (joinRequest.IsAccepted.Value)
                {
                    throw new InvalidOperationException("JoinRequest is already accepted");
                }
                else
                {
                    throw new InvalidOperationException("JoinRequest is already rejected");
                }
            }
            if (joinRequest.Board.CreatorId != userIdPerformingAction)
            {
                throw new UnauthorizedAccessException("You cannot accept or reject join request if you are not creator of the board");
            }
        }

        public JoinRequest RejectJoin(string joinRequestId, string userIdPerformingAction)
        {
            JoinRequest joinRequest = dbContext.JoinRequests.Include(x => x.Board).FirstOrDefault(x => x.Id == joinRequestId);
            ValidateJoinRequestBeforeDecision(userIdPerformingAction, joinRequest);

            joinRequest.Reject(userIdPerformingAction, timeProvider.Now());

            return joinRequest;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
