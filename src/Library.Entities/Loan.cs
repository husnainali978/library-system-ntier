namespace Library.Entities;

/// <summary>
/// Represents a single checkout of one book copy by one member.
/// </summary>
public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public Book? Book { get; set; }

    public int MemberId { get; set; }

    public Member? Member { get; set; }

    public DateTime CheckoutDate { get; set; }

    /// <summary>
    /// CheckoutDate + 14 days, set at checkout time.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Null while the book is still checked out.
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    /// <summary>
    /// Overdue fine accrued for this loan, in dollars, calculated at return time
    /// (or on demand for a still-open loan) at $0.50/day overdue.
    /// </summary>
    public decimal FineAmount { get; set; }

    public bool IsReturned => ReturnDate.HasValue;
}
