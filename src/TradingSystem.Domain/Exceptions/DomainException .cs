using System;

namespace TradingSystem.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    //public DomainException(ErrorCode errorCode, string message) : base(message) { }
}