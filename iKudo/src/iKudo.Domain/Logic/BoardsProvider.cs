using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
