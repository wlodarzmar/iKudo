using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace iKudo.Domain.Interfaces
{
    public interface IManageBoards
    {
        Board Add(Board board);

        void Delete(string userId, int id);

        void Update(Board board);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boardId"></param>
        /// <param name="emails"></param>
        /// <returns>List of keys: mail, and values: http status code</returns>
        Task<List<KeyValuePair<string, HttpStatusCode>>> Invite(string userId, int boardId, string[] emails);

        void AcceptInvitation(string userId, int boardId, string code);
    }
}
