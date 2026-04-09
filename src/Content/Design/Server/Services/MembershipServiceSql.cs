using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Services;

public class MembershipServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : IMembershipService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Membership>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Membership>>(request, async response =>
        {
            var memberships = await MembershipSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return memberships;
        });
    }

    public async Task<Response<Membership?>> Fetch(Request<Membership> request)
    {
        return await wrappers.Try<Membership?>(request, async response =>
        {
            var membership = await MembershipSelect.Execute(Connection, request.SessionId, request.Value);

            return membership;
        });
    }

    public async Task<Response<Membership?>> Add(Request<Membership> request)
    {
        return await wrappers.Validate<Membership?, Membership>(request, async response =>
        {
            var membership = request.Value;

            membership.Description = htmlSanitizer.Sanitize(membership.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await MembershipInsert.Execute(connection, transaction, request.SessionId, membership);

                return new Membership
                {
                    Id = id,
                    PortalId = membership.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Membership> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var membership = request.Value;

            membership.Description = htmlSanitizer.Sanitize(membership.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MembershipUpdate.Execute(connection, transaction, request.SessionId, membership);
            });
        });
    }

    public async Task<Response> Remove(Request<Membership> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var membership = request.Value;
            var existing = await MembershipSelect.Execute(Connection, request.SessionId, membership);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MembershipDelete.Execute(connection, transaction, request.SessionId, membership);
            });
        });
    }

    public async Task<Response> CreateMembership(Request<Membership> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var membership = request.Value;
            var isMoreSignup = membership.Name?.Trim().Equals("MORE", StringComparison.OrdinalIgnoreCase) == true
                               || membership.Name?.Trim().Equals("MORE Signups", StringComparison.OrdinalIgnoreCase) == true;
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                if (isMoreSignup)
                    await MembershipCreateMoreSignups.Execute(connection, transaction, request.SessionId, membership);
                else
                    await MembershipCreate.Execute(connection, transaction, request.SessionId, membership);
            });
        });
    }

    public async Task<Response> CreateTokens(Request<Membership> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var membership = request.Value;
            var isMoreSignup = membership.Name?.Trim().Equals("MORE", StringComparison.OrdinalIgnoreCase) == true
                               || membership.Name?.Trim().Equals("MORE Signups", StringComparison.OrdinalIgnoreCase) == true;
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                if (isMoreSignup)
                    await TokenCreateMoreSignups.Execute(connection, transaction, request.SessionId, membership);
                else
                    await TokenCreate.Execute(connection, transaction, request.SessionId, membership);
            });
        });
    }
}