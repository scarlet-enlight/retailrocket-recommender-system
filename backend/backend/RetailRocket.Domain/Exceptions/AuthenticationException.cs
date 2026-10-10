namespace RetailRocket.Domain.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException(string errorMessage) : base(errorMessage) {}
}