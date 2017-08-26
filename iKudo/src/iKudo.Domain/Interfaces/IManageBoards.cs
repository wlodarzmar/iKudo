﻿using System.Collections.Generic;
using iKudo.Domain.Model;
using iKudo.Domain.Criteria;

namespace iKudo.Domain.Interfaces
{
    public interface IManageBoards
    {
        Board Add(Board board);

        Board Get(int id);

        ICollection<Board> GetAll(BoardSearchCriteria criteria);

        void Delete(string userId, int id);

        void Update(Board board);
    }
}
