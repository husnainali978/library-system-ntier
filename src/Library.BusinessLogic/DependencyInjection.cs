using Library.BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.BusinessLogic;

/// <summary>
/// Registers the Business Logic Layer's own services. Library.API only
/// needs to call this (and Library.DataAccess.AddDataAccess) to wire up
/// the whole application - it never new()'s up a repository or DbContext itself.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ILoanService, LoanService>();

        return services;
    }
}
