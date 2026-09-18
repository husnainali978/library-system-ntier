namespace Library.API.Models;

public record CreateBookRequest(string Title, string Author, string Isbn, int TotalCopies);

public record UpdateBookRequest(string Title, string Author, string Isbn, int TotalCopies);
