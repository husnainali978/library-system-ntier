using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess;

/// <summary>
/// Static seed data so a freshly-created database is immediately useful for
/// demoing the API without any manual setup.
/// </summary>
internal static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", TotalCopies = 3, AvailableCopies = 3 },
            new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "David Thomas & Andrew Hunt", Isbn = "9780135957059", TotalCopies = 2, AvailableCopies = 2 },
            new Book { Id = 3, Title = "Domain-Driven Design", Author = "Eric Evans", Isbn = "9780321125217", TotalCopies = 1, AvailableCopies = 1 },
            new Book { Id = 4, Title = "Design Patterns", Author = "Erich Gamma et al.", Isbn = "9780201633610", TotalCopies = 2, AvailableCopies = 2 }
        );

        modelBuilder.Entity<Member>().HasData(
            new Member { Id = 1, FullName = "Alice Johnson", Email = "alice.johnson@example.com", MembershipDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Member { Id = 2, FullName = "Brian Lee", Email = "brian.lee@example.com", MembershipDate = new DateTime(2024, 3, 22, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
