namespace WebApiPatterns.Exceptions
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string error) : base(error) { }

    }
}
