using Library.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.DataAccess;

/// <summary>
/// Registers the Data Access Layer's own services (the SQLite-backed
/// <see cref="LibraryDbContext"/> and the repositories) so the composition
/// root (Library.API's Program.cs) doesn't need to know DAL internals.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LibraryDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();

        return services;
    }
}
