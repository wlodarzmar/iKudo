using System;

namespace iKudo.Domain
{
    public class CompanyAlreadyExistException : Exception
    {
        public CompanyAlreadyExistException(string message) : base(message)
        {
        }
    }
}
