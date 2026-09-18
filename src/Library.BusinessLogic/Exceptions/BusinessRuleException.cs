namespace Library.BusinessLogic.Exceptions;

/// <summary>
/// Thrown when an operation would violate one of the library's lending rules
/// (e.g. no copies available, member has reached the borrowing limit).
/// The API layer maps this to an HTTP 400 response.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

/// <summary>
/// Thrown when a requested entity (book, member, loan) does not exist.
/// The API layer maps this to an HTTP 404 response.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
