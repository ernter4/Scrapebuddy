using System;

namespace ScrapeBuddy.Exceptions
{
    /// <summary>
    /// Base exception for ScrapeBuddy.
    /// </summary>
    public class BaseException : Exception
    {
        public BaseException(string message) : base(message) {}
        public BaseException(string message, Exception innerException) : base(message, innerException) {}
        public BaseException() : base() {}
    }
}