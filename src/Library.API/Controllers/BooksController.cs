using Library.API.Models;
using Library.BusinessLogic.Services;
using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

/// <summary>
/// Thin controller for the book catalog. Every action simply delegates to
/// <see cref="IBookService"/> in the Business Logic Layer - no data access
/// code lives here.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Book>>> GetAll()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _bookService.GetBookAsync(id);
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create(CreateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            TotalCopies = request.TotalCopies,
        };

        var created = await _bookService.AddBookAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            TotalCopies = request.TotalCopies,
        };

        await _bookService.UpdateBookAsync(id, book);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return NoContent();
    }
}
