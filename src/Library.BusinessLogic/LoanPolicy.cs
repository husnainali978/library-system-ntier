namespace Library.BusinessLogic;

/// <summary>
/// Centralizes the library's lending business rules as named constants so
/// they are defined in exactly one place and are easy to reason about /
/// change without hunting through service methods.
/// </summary>
public static class LoanPolicy
{
    /// <summary>
    /// Number of days a member may keep a book before it is due.
    /// </summary>
    public const int LoanPeriodDays = 14;

    /// <summary>
    /// Fine accrued per day (or part of a day) a book is returned late.
    /// </summary>
    public const decimal DailyFineRate = 0.50m;

    /// <summary>
    /// Maximum number of books a single member may have checked out at once.
    /// </summary>
    public const int MaxActiveLoansPerMember = 5;
}
