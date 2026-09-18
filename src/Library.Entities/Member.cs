namespace Library.Entities;

/// <summary>
/// Represents a person registered to borrow books from the library.
/// </summary>
public class Member
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime MembershipDate { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
