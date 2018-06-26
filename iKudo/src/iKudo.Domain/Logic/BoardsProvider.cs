using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.String;

namespace iKudo.Domain.Logic
{
    public class BoardsProvider : IProvideBoards
    {
        private readonly KudoDbContext dbContext;

        public BoardsProvider(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Board Get(int id)
        {
            return dbContext.Boards.Include(x => x.UserBoards).FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Board> GetAll(string userId, BoardSearchCriteria criteria)
        {
            criteria = criteria ?? new BoardSearchCriteria();
            IQueryable<Board> boards = dbContext.Boards.Where(x => x.IsPublic || x.CreatorId == userId);

            boards = boards.WhereIf(x => x.CreatorId == criteria.CreatorId, !IsNullOrWhiteSpace(criteria.CreatorId));
            boards = boards.WhereIf(x => x.UserBoards.Select(ub => ub.UserId).Contains(criteria.Member), !IsNullOrWhiteSpace(criteria.Member));

            return boards.ToList();
        }
    }

    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> collection, Expression<Func<T, bool>> predicate, bool condition)
        {
            if (condition)
            {
                collection = collection.Where(predicate);
            }

            return collection;
        }
    }
}
