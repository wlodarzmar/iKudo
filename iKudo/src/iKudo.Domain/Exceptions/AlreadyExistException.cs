﻿using System;
using System.Runtime.Serialization;

namespace iKudo.Domain.Exceptions
{
    [Serializable]
    public class AlreadyExistException : KudoException
    {
        public AlreadyExistException(string message) : base(message)
        {
        }

        public AlreadyExistException()
        {
        }

        public AlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
