using System;

namespace iKudo.Domain.Exceptions
{
    public class GroupAlreadyExistException : Exception
    {
        public GroupAlreadyExistException(string message) : base(message)
        {
        }
    }
}
