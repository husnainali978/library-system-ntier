using Library.Entities;

namespace Library.DataAccess.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int id);

    Task<Loan?> GetByIdWithDetailsAsync(int id);

    Task<IReadOnlyList<Loan>> GetAllAsync();

    Task<IReadOnlyList<Loan>> GetActiveLoansForMemberAsync(int memberId);

    Task<IReadOnlyList<Loan>> GetOverdueLoansAsync(DateTime asOf);

    Task<Loan> AddAsync(Loan loan);

    Task UpdateAsync(Loan loan);
}
