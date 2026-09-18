using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _context;

    public MemberRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(int id) =>
        await _context.Members.FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IReadOnlyList<Member>> GetAllAsync() =>
        await _context.Members.AsNoTracking().OrderBy(m => m.FullName).ToListAsync();

    public async Task<Member> AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is not null)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
