using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;

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

            if (board.JoinRequests.Any(x => x.Status == JoinStatus.Waiting && x.CandidateId == candidateId))
            {
                throw new InvalidOperationException("There is not accepted request already");
            }

            if (board.UserBoards.Any(x => x.BoardId == boardId && x.UserId == candidateId))
            {
                throw new InvalidOperationException("User is a member of this board already");
            }

            JoinRequest joinRequest = new JoinRequest(board.Id, candidateId, timeProvider.Now());

            board.JoinRequests.Add(joinRequest);
            Notification joinAddedNotification = new Notification(candidateId, board.CreatorId, timeProvider.Now(), NotificationTypes.BoardJoinAdded)
            {
                BoardId = board.Id
            };
            dbContext.Notifications.Add(joinAddedNotification);

            dbContext.SaveChanges();

            return joinRequest;
        }

        public IEnumerable<JoinRequest> GetJoins(JoinSearchCriteria criteria)
        {
            IQueryable<JoinRequest> joins = dbContext.JoinRequests;

            if (criteria.BoardId.HasValue)
            {
                joins = joins.Where(x => x.BoardId == criteria.BoardId.Value);
            }

            if (criteria.Status.HasValue)
            {
                joins = joins.Where(x => x.Status == criteria.Status);
            }

            if (!string.IsNullOrEmpty(criteria.CandidateId))
            {
                joins = joins.Where(x => x.CandidateId == criteria.CandidateId);
            }

            return joins.ToList();
        }

        public JoinRequest AcceptJoin(int joinRequestId, string userIdPerformingAction)
        {
            JoinRequest joinRequest = dbContext.JoinRequests.Include(x => x.Board).FirstOrDefault(x => x.Id == joinRequestId);
            ValidateJoinRequestBeforeDecision(userIdPerformingAction, joinRequest);

            joinRequest.Accept(userIdPerformingAction, timeProvider.Now());
            dbContext.UserBoards.Add(new UserBoard(joinRequest.CandidateId, joinRequest.BoardId));
            Notification acceptNotification = new Notification(userIdPerformingAction, joinRequest.CandidateId, timeProvider.Now(), NotificationTypes.BoardJoinAccepted)
            {
                BoardId = joinRequest.BoardId
            };
            dbContext.Notifications.Add(acceptNotification);
            dbContext.SaveChanges();

            return joinRequest;
        }

        private static void ValidateJoinRequestBeforeDecision(string userIdPerformingAction, JoinRequest join)
        {
            if (join == null)
            {
                throw new NotFoundException("JoinRequest with given id does not exist");
            }

            if (join.Status == JoinStatus.Accepted)
            {
                throw new InvalidOperationException("JoinRequest is already accepted");
            }
            else if (join.Status == JoinStatus.Rejected)
            {
                throw new InvalidOperationException("JoinRequest is already rejected");
            }

            if (join.Board.CreatorId != userIdPerformingAction)
            {
                throw new UnauthorizedAccessException("You cannot accept or reject join request if you are not creator of the board");
            }
        }

        public JoinRequest RejectJoin(int joinRequestId, string userIdPerformingAction)
        {
            JoinRequest joinRequest = dbContext.JoinRequests.Include(x => x.Board).FirstOrDefault(x => x.Id == joinRequestId);
            ValidateJoinRequestBeforeDecision(userIdPerformingAction, joinRequest);

            joinRequest.Reject(userIdPerformingAction, timeProvider.Now());
            Notification rejectNotification = new Notification(userIdPerformingAction, joinRequest.CandidateId, timeProvider.Now(), NotificationTypes.BoardJoinRejected)
            {
                BoardId = joinRequest.BoardId
            };
            dbContext.Notifications.Add(rejectNotification);
            dbContext.SaveChanges();

            return joinRequest;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
