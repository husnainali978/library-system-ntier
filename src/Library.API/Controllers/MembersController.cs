using Library.API.Models;
using Library.BusinessLogic.Services;
using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

/// <summary>
/// Thin controller for library members - delegates all logic to
/// <see cref="IMemberService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetAll()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Member>> GetById(int id)
    {
        var member = await _memberService.GetMemberAsync(id);
        return Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<Member>> Register(RegisterMemberRequest request)
    {
        var member = new Member
        {
            FullName = request.FullName,
            Email = request.Email,
        };

        var created = await _memberService.RegisterMemberAsync(member);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _memberService.DeleteMemberAsync(id);
        return NoContent();
    }
}
