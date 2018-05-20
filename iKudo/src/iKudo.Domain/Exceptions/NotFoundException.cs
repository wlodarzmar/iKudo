using System;
using System.Runtime.Serialization;

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

        protected NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}