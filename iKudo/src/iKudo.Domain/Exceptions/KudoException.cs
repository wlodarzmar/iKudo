using System;
using System.Runtime.Serialization;

namespace iKudo.Domain.Exceptions
{
    public abstract class KudoException : Exception
    {
        public KudoException()
        {
        }

        public KudoException(string message) : base(message)
        {
        }

        public KudoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KudoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
