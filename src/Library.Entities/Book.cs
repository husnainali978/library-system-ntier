namespace Library.Entities;

/// <summary>
/// Represents a book title held by the library, along with its copy inventory.
/// A single <see cref="Book"/> record represents a title, not a physical item -
/// <see cref="TotalCopies"/> tracks how many physical copies the library owns
/// and <see cref="AvailableCopies"/> tracks how many are currently on the shelf.
/// </summary>
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    /// <summary>
    /// Total number of physical copies the library owns for this title.
    /// </summary>
    public int TotalCopies { get; set; }

    /// <summary>
    /// Number of copies currently on the shelf (not checked out).
    /// </summary>
    public int AvailableCopies { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
