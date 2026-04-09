namespace Crudspa.Content.Design.Server.Services;

public class StyleServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IStyleService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Style>>> FetchForContentPortal(Request<ContentPortal> request)
    {
        return await wrappers.Try<IList<Style>>(request, async response =>
        {
            var styles = await StyleSelectForContentPortal.Execute(Connection, request.SessionId, request.Value.Id);
            return styles;
        });
    }

    public async Task<Response<Style?>> Fetch(Request<Style> request)
    {
        return await wrappers.Try<Style?>(request, async response =>
        {
            var style = await StyleSelect.Execute(Connection, request.SessionId, request.Value);
            return style;
        });
    }

    public async Task<Response> Save(Request<Style> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var style = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StyleUpdate.Execute(connection, transaction, request.SessionId, style);
            });
        });
    }

    public async Task<Response<IList<RuleFull>>> FetchRules(Request request)
    {
        return await wrappers.Try<IList<RuleFull>>(request, async response =>
            await RuleSelectFull.Execute(Connection));
    }
}