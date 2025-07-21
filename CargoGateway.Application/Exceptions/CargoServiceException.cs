namespace CargoGateway.Core.Exceptions;

public class CargoServiceException : Exception
{
    public CargoServiceException() { }
    public CargoServiceException(string message) : base(message) { }
    public CargoServiceException(string message, Exception inner) 
        : base(message, inner) { }
}