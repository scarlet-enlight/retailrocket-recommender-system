namespace RetailRocket.Domain.Exceptions;

public class BannedUserException : Exception
{
    public BannedUserException(string errorMessage) : base(errorMessage) {}
}