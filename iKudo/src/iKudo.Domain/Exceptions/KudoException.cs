using System;
using System.Runtime.Serialization;

namespace iKudo.Domain.Exceptions
{
    [Serializable]
    public class KudoException : Exception
    {
        public KudoException()
        {
        }

        public KudoException(string message) : base(message)
        {
        }

        protected KudoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KudoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
