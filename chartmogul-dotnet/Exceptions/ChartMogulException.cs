using System;
namespace chartmoguldotnet.Exceptions
{
    public class ChartMogulException : Exception
    {
        public ChartMogulException()
        {
        }

        public ChartMogulException(string message)
            : base(message)
        {
        }

        public ChartMogulException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

