namespace iKudo.Domain.Exceptions
{
    public class NotFoundException : KudoException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}