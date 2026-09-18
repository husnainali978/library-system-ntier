using Library.Entities;

namespace Library.BusinessLogic.Services;

public interface ILoanService
{
    /// <summary>
    /// Checks out one copy of a book to a member, enforcing availability and
    /// the max-active-loans-per-member rule. Due date is set to checkout + 14 days.
    /// </summary>
    Task<Loan> CheckoutBookAsync(int memberId, int bookId);

    /// <summary>
    /// Returns a checked-out book, calculating any overdue fine at $0.50/day.
    /// </summary>
    Task<Loan> ReturnBookAsync(int loanId);

    Task<Loan> GetLoanAsync(int loanId);

    Task<IReadOnlyList<Loan>> GetAllLoansAsync();

    Task<IReadOnlyList<Loan>> GetActiveLoansForMemberAsync(int memberId);

    Task<IReadOnlyList<Loan>> GetOverdueLoansAsync();

    /// <summary>
    /// Calculates the fine a loan has accrued as of now, without returning it.
    /// Useful for showing a member how much they currently owe.
    /// </summary>
    Task<decimal> CalculateCurrentFineAsync(int loanId);
}
