using System;

namespace iKudo.Domain.Exceptions
{
    [Serializable]
    public class AlreadyExistException : KudoException
    {
        public AlreadyExistException(string message) : base(message)
        {
        }
    }
}
