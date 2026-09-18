using Library.Entities;

namespace Library.DataAccess.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);

    Task<IReadOnlyList<Book>> GetAllAsync();

    Task<Book> AddAsync(Book book);

    Task UpdateAsync(Book book);

    Task DeleteAsync(int id);

    Task SaveChangesAsync();
}
