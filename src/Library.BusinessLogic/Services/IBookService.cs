using Library.Entities;

namespace Library.BusinessLogic.Services;

public interface IBookService
{
    Task<IReadOnlyList<Book>> GetAllBooksAsync();

    Task<Book> GetBookAsync(int id);

    Task<Book> AddBookAsync(Book book);

    Task UpdateBookAsync(int id, Book book);

    Task DeleteBookAsync(int id);
}
