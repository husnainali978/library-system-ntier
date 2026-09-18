using Library.API.Models;
using Library.BusinessLogic.Services;
using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

/// <summary>
/// Thin controller for checkout/return and loan queries - delegates all
/// lending rules (due dates, fines, borrowing limits) to
/// <see cref="ILoanService"/> in the Business Logic Layer.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Loan>>> GetAll()
    {
        var loans = await _loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Loan>> GetById(int id)
    {
        var loan = await _loanService.GetLoanAsync(id);
        return Ok(loan);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<Loan>>> GetOverdue()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        return Ok(loans);
    }

    [HttpGet("member/{memberId:int}/active")]
    public async Task<ActionResult<IReadOnlyList<Loan>>> GetActiveForMember(int memberId)
    {
        var loans = await _loanService.GetActiveLoansForMemberAsync(memberId);
        return Ok(loans);
    }

    [HttpGet("{id:int}/fine")]
    public async Task<ActionResult<decimal>> GetCurrentFine(int id)
    {
        var fine = await _loanService.CalculateCurrentFineAsync(id);
        return Ok(fine);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<Loan>> Checkout(CheckoutRequest request)
    {
        var loan = await _loanService.CheckoutBookAsync(request.MemberId, request.BookId);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<Loan>> Return(int id)
    {
        var loan = await _loanService.ReturnBookAsync(id);
        return Ok(loan);
    }
}
