using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetByIdAsync(int id) =>
        await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IReadOnlyList<Book>> GetAllAsync() =>
        await _context.Books.AsNoTracking().OrderBy(b => b.Title).ToListAsync();

    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is not null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
