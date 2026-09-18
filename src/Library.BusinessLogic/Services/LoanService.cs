using Library.BusinessLogic.Exceptions;
using Library.DataAccess.Repositories;
using Library.Entities;

namespace Library.BusinessLogic.Services;

/// <summary>
/// Core lending business logic: checkout, return, due-date calculation,
/// overdue-fine calculation, and the max-books-per-member rule.
/// This is the heart of the N-Tier "Business Logic Layer" for this system.
/// </summary>
public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;

    public LoanService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Loan> CheckoutBookAsync(int memberId, int bookId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId)
            ?? throw new NotFoundException($"Member {memberId} was not found.");

        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException($"Book {bookId} was not found.");

        if (book.AvailableCopies < 1)
        {
            throw new BusinessRuleException($"'{book.Title}' has no copies available right now.");
        }

        var activeLoans = await _loanRepository.GetActiveLoansForMemberAsync(memberId);
        if (activeLoans.Count >= LoanPolicy.MaxActiveLoansPerMember)
        {
            throw new BusinessRuleException(
                $"{member.FullName} already has {LoanPolicy.MaxActiveLoansPerMember} books checked out, " +
                "which is the maximum allowed.");
        }

        if (activeLoans.Any(l => l.BookId == bookId))
        {
            throw new BusinessRuleException($"{member.FullName} already has a copy of '{book.Title}' checked out.");
        }

        var checkoutDate = DateTime.UtcNow;
        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            CheckoutDate = checkoutDate,
            DueDate = checkoutDate.AddDays(LoanPolicy.LoanPeriodDays),
            ReturnDate = null,
            FineAmount = 0m,
        };

        book.AvailableCopies -= 1;
        await _bookRepository.UpdateAsync(book);

        return await _loanRepository.AddAsync(loan);
    }

    public async Task<Loan> ReturnBookAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId)
            ?? throw new NotFoundException($"Loan {loanId} was not found.");

        if (loan.IsReturned)
        {
            throw new BusinessRuleException("This loan has already been returned.");
        }

        var returnDate = DateTime.UtcNow;
        loan.ReturnDate = returnDate;
        loan.FineAmount = CalculateOverdueFine(loan.DueDate, returnDate);

        var book = await _bookRepository.GetByIdAsync(loan.BookId);
        if (book is not null)
        {
            book.AvailableCopies = Math.Min(book.TotalCopies, book.AvailableCopies + 1);
            await _bookRepository.UpdateAsync(book);
        }

        await _loanRepository.UpdateAsync(loan);
        return loan;
    }

    public async Task<Loan> GetLoanAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdWithDetailsAsync(loanId);
        return loan ?? throw new NotFoundException($"Loan {loanId} was not found.");
    }

    public Task<IReadOnlyList<Loan>> GetAllLoansAsync() => _loanRepository.GetAllAsync();

    public Task<IReadOnlyList<Loan>> GetActiveLoansForMemberAsync(int memberId) =>
        _loanRepository.GetActiveLoansForMemberAsync(memberId);

    public Task<IReadOnlyList<Loan>> GetOverdueLoansAsync() =>
        _loanRepository.GetOverdueLoansAsync(DateTime.UtcNow);

    public async Task<decimal> CalculateCurrentFineAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId)
            ?? throw new NotFoundException($"Loan {loanId} was not found.");

        return loan.IsReturned
            ? loan.FineAmount
            : CalculateOverdueFine(loan.DueDate, DateTime.UtcNow);
    }

    /// <summary>
    /// Pure fine calculation: $0.50 for every full day between the due date
    /// and the (actual or hypothetical) return date. Returns 0 if not overdue.
    /// </summary>
    private static decimal CalculateOverdueFine(DateTime dueDate, DateTime returnDate)
    {
        if (returnDate <= dueDate)
        {
            return 0m;
        }

        var daysLate = Math.Ceiling((returnDate - dueDate).TotalDays);
        return (decimal)daysLate * LoanPolicy.DailyFineRate;
    }
}
