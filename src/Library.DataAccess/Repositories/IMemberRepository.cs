using Library.Entities;

namespace Library.DataAccess.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(int id);

    Task<IReadOnlyList<Member>> GetAllAsync();

    Task<Member> AddAsync(Member member);

    Task UpdateAsync(Member member);

    Task DeleteAsync(int id);
}
