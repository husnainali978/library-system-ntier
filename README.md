# Library Book Lending System — N-Tier Architecture

A library lending API built to demonstrate classic **N-Tier (layered) architecture**
in .NET — the layering style most enterprise line-of-business systems have used
for the last two decades, and still the one you're most likely to walk into on
the job. It's deliberately distinct from Clean/Onion Architecture: instead of
Domain/Application/Infrastructure rings with dependency inversion at the center,
N-Tier uses straightforward, unidirectional layers named after their physical
role (Presentation, Business Logic, Data Access) with concrete top-to-bottom
references. No abstraction-for-abstraction's-sake — just a clean separation of
concerns that's fast to read and easy to onboard onto.

## Architecture

Four projects, each depending only on the project directly beneath it:

```
Library.API            (Presentation Tier — ASP.NET Core Web API)
      |
      v
Library.BusinessLogic   (Business Logic Layer — rules, calculations, validation)
      |
      v
Library.DataAccess      (Data Access Layer — EF Core DbContext + repositories)
      |
      v
Library.Entities         (plain entity/model classes shared by every layer)
```

- **Library.Entities** — `Book`, `Member`, `Loan`. Plain classes with no
  framework dependencies; every other layer builds on these.
- **Library.DataAccess** — `LibraryDbContext` (EF Core, SQLite) plus a
  repository per entity (`IBookRepository`, `IMemberRepository`,
  `ILoanRepository`). This is the only layer that knows SQL/EF exists.
- **Library.BusinessLogic** — `BookService`, `MemberService`, `LoanService`.
  Owns the lending rules: due-date calculation, overdue-fine calculation,
  and the max-books-per-member limit. Talks to `DataAccess` through
  repository interfaces only.
- **Library.API** — Thin ASP.NET Core controllers. Each action is a couple of
  lines that call into a `BusinessLogic` service and return the result —
  no EF, no SQL, no direct `DataAccess` reference at all.

The reference direction is enforced with real `<ProjectReference>` entries,
not just a naming convention: `Library.API.csproj` only references
`Library.BusinessLogic`, which only references `Library.DataAccess`, which
only references `Library.Entities`. Because project references are
transitive, `Library.API` still has compile-time access to `Book`/`Member`/`Loan`
types without ever being allowed to reference `Library.DataAccess` directly —
so a controller physically cannot call a repository or touch `DbContext`.

## Features

- **Book catalog** — add, update, remove, and browse titles, each tracking
  `TotalCopies` (owned) vs. `AvailableCopies` (currently on the shelf).
- **Members** — register library members and look them up.
- **Checkout** — borrowing a book:
  - fails if the book has no available copies
  - fails if the member already has 5 active loans
  - fails if the member already has that exact title checked out
  - sets `DueDate = CheckoutDate + 14 days`
  - decrements the book's available-copy count
- **Return** — returning a loan:
  - calculates a $0.50/day fine for every day past the due date
  - restores the book's available-copy count
- **Overdue tracking** — list every loan currently past its due date, or
  preview the fine a loan has accrued so far without returning it.
- Business-rule violations (e.g. no copies left) return HTTP 400; missing
  entities return HTTP 404 — translated centrally by an exception-handling
  middleware so controllers stay free of try/catch.
- SQLite database is created automatically on first run and seeded with a
  handful of books and members.

## How to run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# from the repository root
dotnet restore
dotnet build

# run the API (SQLite database is created + seeded automatically)
dotnet run --project src/Library.API
```

The API listens on the URL printed in the console (see
`src/Library.API/Properties/launchSettings.json`). Once it's running, open
`src/Library.API/Library.API.http` in an editor with REST Client support
(e.g. VS Code's REST Client extension, or Visual Studio/Rider's built-in
`.http` runner) and try the example requests — checkout a book, then hit the
return endpoint to see the fine calculation in action.

## Tech stack

.NET 10, ASP.NET Core Web API, Entity Framework Core (SQLite provider), C#.
