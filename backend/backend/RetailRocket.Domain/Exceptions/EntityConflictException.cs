namespace RetailRocket.Domain.Exceptions;

public class EntityConflictException : Exception
{
    public EntityConflictException(string errorMessage) : base(errorMessage) {}
}