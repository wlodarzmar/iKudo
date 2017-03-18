namespace iKudo.Controllers.Api
{
    public class Result
    {
        public Result(string error)
        {
            Error = error;
        }

        public string Error { get; set; }
    }
}
