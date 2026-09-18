using Library.Entities;

namespace Library.BusinessLogic.Services;

public interface IMemberService
{
    Task<IReadOnlyList<Member>> GetAllMembersAsync();

    Task<Member> GetMemberAsync(int id);

    Task<Member> RegisterMemberAsync(Member member);

    Task DeleteMemberAsync(int id);
}
