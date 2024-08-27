using System;
using Tmds.DBus.Protocol;

namespace ScrapeBuddy.Exceptions;

/// <summary>Represents a base exception for Selenium-related errors.</summary>
public class SeleniumException:BaseException

{
    public SeleniumException(string message,Exception innerException ):base(message,innerException){}
    public SeleniumException(string message):base(message){}
    public SeleniumException():base(){}
}

/// <summary>Represents an exception for login failures.</summary>
public class LoginException : SeleniumException
{
    public LoginException(string message, Exception innerException) : base(message, innerException) {}
    public LoginException(string message) : base(message) {}
    public LoginException() : base() {}

}

/// <summary>Represents an exception for missing username field during login.</summary>
public class MissingUsernamefieldException : LoginException

{
    public MissingUsernamefieldException(string message, Exception innerException) : base(message, innerException) {}
    public MissingUsernamefieldException(string message) : base(message) {}
    public MissingUsernamefieldException() : base() {}
}

/// <summary>Represents an exception for missing password field during login.</summary>
public class MissingPasswordfieldException : LoginException
{
    public MissingPasswordfieldException(string message,Exception innerException):base(message,innerException) {}
    public MissingPasswordfieldException(string message):base(message) {}
    public MissingPasswordfieldException():base() {}
}