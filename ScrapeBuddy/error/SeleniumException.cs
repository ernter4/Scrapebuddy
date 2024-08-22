using System;
using Tmds.DBus.Protocol;

namespace ScrapeBuddy.Error;

public class SeleniumException:BaseException

{
    public SeleniumException(string message,Exception innerException ):base(message,innerException){}
    public SeleniumException(string message):base(message){}
    public SeleniumException():base(){}
}

public class LoginException : SeleniumException
{
    public LoginException(string message, Exception innerException) : base(message, innerException) {}
    public LoginException(string message) : base(message) {}
    public LoginException() : base() {}

}

public class MissingUsernamefieldException : LoginException

{
    public MissingUsernamefieldException(string message, Exception innerException) : base(message, innerException) {}
    public MissingUsernamefieldException(string message) : base(message) {}
    public MissingUsernamefieldException() : base() {}
}

public class MissingPasswordfieldException : LoginException
{
    public MissingPasswordfieldException(string message,Exception innerException):base(message,innerException) {}
    public MissingPasswordfieldException(string message):base(message) {}
    public MissingPasswordfieldException():base() {}
}