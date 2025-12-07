namespace WebApiPatterns.Exceptions
{
    public class MultipleHandlersException : Exception
    {
        public MultipleHandlersException(string error):base(error) { } 
    }
}
