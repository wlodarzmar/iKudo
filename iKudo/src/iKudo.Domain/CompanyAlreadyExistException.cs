using System;

namespace iKudo.Domain.Exceptions
{
    public class CompanyAlreadyExistException : Exception
    {
        public CompanyAlreadyExistException(string message) : base(message)
        {
        }
    }
}
