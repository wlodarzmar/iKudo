namespace iKudo.Controllers.Api
{
    public class ErrorResult
    {
        public ErrorResult(string error)
        {
            Error = error;
        }

        public string Error { get; set; }
    }
}
