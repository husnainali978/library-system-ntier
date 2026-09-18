using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _context;

    public LoanRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(int id) =>
        await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loan?> GetByIdWithDetailsAsync(int id) =>
        await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IReadOnlyList<Loan>> GetAllAsync() =>
        await _context.Loans
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.CheckoutDate)
            .ToListAsync();

    public async Task<IReadOnlyList<Loan>> GetActiveLoansForMemberAsync(int memberId) =>
        await _context.Loans
            .Where(l => l.MemberId == memberId && l.ReturnDate == null)
            .Include(l => l.Book)
            .ToListAsync();

    public async Task<IReadOnlyList<Loan>> GetOverdueLoansAsync(DateTime asOf) =>
        await _context.Loans
            .AsNoTracking()
            .Where(l => l.ReturnDate == null && l.DueDate < asOf)
            .Include(l => l.Book)
            .Include(l => l.Member)
            .ToListAsync();

    public async Task<Loan> AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }
}
