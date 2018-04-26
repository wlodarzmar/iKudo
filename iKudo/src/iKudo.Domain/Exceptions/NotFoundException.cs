using System;

namespace iKudo.Domain.Exceptions
{
    [Serializable]
    public class NotFoundException : KudoException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}