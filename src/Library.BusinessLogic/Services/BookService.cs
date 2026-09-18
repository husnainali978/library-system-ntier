using Library.BusinessLogic.Exceptions;
using Library.DataAccess.Repositories;
using Library.Entities;

namespace Library.BusinessLogic.Services;

/// <summary>
/// Business logic for managing the book catalog. Beyond simple CRUD
/// delegation, this is where catalog-level rules (e.g. copy-count
/// validation) live.
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public Task<IReadOnlyList<Book>> GetAllBooksAsync() => _bookRepository.GetAllAsync();

    public async Task<Book> GetBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book ?? throw new NotFoundException($"Book {id} was not found.");
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
        {
            throw new BusinessRuleException("Book title is required.");
        }

        if (book.TotalCopies < 1)
        {
            throw new BusinessRuleException("A book must have at least one copy.");
        }

        // A newly-catalogued book starts with every copy on the shelf.
        book.AvailableCopies = book.TotalCopies;

        return await _bookRepository.AddAsync(book);
    }

    public async Task UpdateBookAsync(int id, Book book)
    {
        var existing = await _bookRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Book {id} was not found.");

        if (book.TotalCopies < 0)
        {
            throw new BusinessRuleException("Total copies cannot be negative.");
        }

        var copiesOnLoan = existing.TotalCopies - existing.AvailableCopies;
        if (book.TotalCopies < copiesOnLoan)
        {
            throw new BusinessRuleException(
                $"Cannot reduce total copies below {copiesOnLoan}, the number currently on loan.");
        }

        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.Isbn = book.Isbn;
        existing.AvailableCopies += book.TotalCopies - existing.TotalCopies;
        existing.TotalCopies = book.TotalCopies;

        await _bookRepository.UpdateAsync(existing);
    }

    public async Task DeleteBookAsync(int id)
    {
        var existing = await _bookRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Book {id} was not found.");

        if (existing.AvailableCopies != existing.TotalCopies)
        {
            throw new BusinessRuleException("Cannot delete a book that has copies currently on loan.");
        }

        await _bookRepository.DeleteAsync(id);
    }
}
