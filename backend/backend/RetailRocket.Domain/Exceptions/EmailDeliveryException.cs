namespace RetailRocket.Domain.Exceptions;

public class EmailDeliveryException :  Exception
{ 
    public EmailDeliveryException(string errorMessage) : base(errorMessage) {}
}