namespace TradingSystem.Domain.Exceptions
{
    public class InvalidInstrumentException : DomainException
    {
        public InvalidInstrumentException(string message) : base(message) { }
    }
}