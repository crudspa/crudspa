using MemberSearch = Crudspa.Content.Design.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Design.Server.Services;

public class MemberServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IMemberService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Member>>> SearchForMembership(Request<MemberSearch> request)
    {
        return await wrappers.Try<IList<Member>>(request, async response =>
        {
            var members = await MemberSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return members;
        });
    }

    public async Task<Response<Member?>> Fetch(Request<Member> request)
    {
        return await wrappers.Try<Member?>(request, async response =>
        {
            var member = await MemberSelect.Execute(Connection, request.SessionId, request.Value);

            return member;
        });
    }

    public async Task<Response<Member?>> Add(Request<Member> request)
    {
        return await wrappers.Validate<Member?, Member>(request, async response =>
        {
            var member = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await MemberInsert.Execute(connection, transaction, request.SessionId, member);

                return new Member
                {
                    Id = id,
                    MembershipId = member.MembershipId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Member> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var member = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MemberUpdate.Execute(connection, transaction, request.SessionId, member);
            });
        });
    }

    public async Task<Response> Remove(Request<Member> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var member = request.Value;
            var existing = await MemberSelect.Execute(Connection, request.SessionId, member);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MemberDelete.Execute(connection, transaction, request.SessionId, member);
            });
        });
    }
}