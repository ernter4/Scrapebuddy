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

public class ScrapeFailedException : SeleniumException
{
    public ScrapeFailedException(string message, Exception innerException) : base(
        message + ": " + innerException.Message, innerException) { }
    public ScrapeFailedException(string message) : base(message) { }
}

/// <summary>Represents an exception for login failures.</summary>
public class LoginException : ScrapeFailedException
{
    public LoginException(string message):base(message){}
    public LoginException(string message, Exception innerException) : base(message +": " + innerException.Message, innerException) {}
    public LoginException(Exception innerException):base($"Login Failed: {innerException.Message}",innerException){}
    
}

public class MissingWebFieldException :ScrapeFailedException
{
    public MissingWebFieldException(String message):base(message){}
    public  MissingWebFieldException(String message ,Exception innerException):base(message+": "+ innerException.Message,innerException){}
    
}

public class ClickactionfailedException : ScrapeFailedException
{
    public ClickactionfailedException(Exception innerException):base("A click Action failed: " +innerException.Message,innerException) {}
}

/// <summary>Represents an exception for missing username field during login.</summary>
public class MissingUsernamefieldException : LoginException

{
    public MissingUsernamefieldException(string message, Exception innerException) : base(message +": "+ innerException.Message, innerException) {}
    public MissingUsernamefieldException(string message) : base(message) {}
    public MissingUsernamefieldException() : base("No Username field found") {}
}

/// <summary>Represents an exception for missing password field during login.</summary>
public class MissingPasswordfieldException : LoginException
{
    public MissingPasswordfieldException(string message,Exception innerException):base(message+" "+ innerException.Message,innerException) {}
    public MissingPasswordfieldException(string message):base(message) {}
    public MissingPasswordfieldException() : base("No Password field found"){}
    
    
}