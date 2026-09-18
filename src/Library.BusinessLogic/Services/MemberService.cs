using System.Text.RegularExpressions;
using Library.BusinessLogic.Exceptions;
using Library.DataAccess.Repositories;
using Library.Entities;

namespace Library.BusinessLogic.Services;

/// <summary>
/// Business logic for managing library members.
/// </summary>
public partial class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;

    public MemberService(IMemberRepository memberRepository, ILoanRepository loanRepository)
    {
        _memberRepository = memberRepository;
        _loanRepository = loanRepository;
    }

    public Task<IReadOnlyList<Member>> GetAllMembersAsync() => _memberRepository.GetAllAsync();

    public async Task<Member> GetMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        return member ?? throw new NotFoundException($"Member {id} was not found.");
    }

    public async Task<Member> RegisterMemberAsync(Member member)
    {
        if (string.IsNullOrWhiteSpace(member.FullName))
        {
            throw new BusinessRuleException("Member full name is required.");
        }

        if (string.IsNullOrWhiteSpace(member.Email) || !EmailPattern().IsMatch(member.Email))
        {
            throw new BusinessRuleException("A valid email address is required.");
        }

        member.MembershipDate = DateTime.UtcNow;
        return await _memberRepository.AddAsync(member);
    }

    public async Task DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Member {id} was not found.");

        var activeLoans = await _loanRepository.GetActiveLoansForMemberAsync(id);
        if (activeLoans.Count > 0)
        {
            throw new BusinessRuleException("Cannot remove a member with books still checked out.");
        }

        await _memberRepository.DeleteAsync(member.Id);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
