using System;

namespace ScrapeBuddy.Error;

public class BaseException:Exception
{
    public BaseException(string message,Exception innerException) :base(message,innerException){}
    public BaseException(string message) :base(message){}
    public BaseException() : base(){}



}