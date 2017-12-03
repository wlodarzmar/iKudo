namespace iKudo.Domain.Exceptions
{
    public class AlreadyExistException : KudoException
    {
        public AlreadyExistException(string message) : base(message)
        {
        }
    }
}
