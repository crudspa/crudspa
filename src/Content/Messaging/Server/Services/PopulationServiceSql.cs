namespace Crudspa.Content.Messaging.Server.Services;

public class PopulationServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    IEnumerable<IPopulationResolver> populationResolvers)
    : IPopulationService
{
    private readonly PopulationResolverRegistry _resolverRegistry = new(populationResolvers);
    private String Connection => configService.Fetch().Database;

    public async Task<Response<PopulationRefreshResult?>> Refresh(Request<PopulationRefresh> request)
    {
        return await wrappers.Validate<PopulationRefreshResult?, PopulationRefresh>(request, async response =>
        {
            var context = await PopulationContextSelect.Execute(Connection, request.SessionId, request.Value);

            if (context is null)
                throw new InvalidOperationException("The Population context could not be resolved.");

            if (context.Population.ResolverKey.HasNothing())
                throw new InvalidOperationException("The Population has no resolver.");

            context.MembershipId ??= await PopulationMembershipEnsure.Execute(
                Connection,
                request.SessionId,
                request.Value.PopulationId,
                request.Value.OrganizationId,
                request.Value.ActivationScopeId);

            if (context.MembershipId is null)
                throw new InvalidOperationException("The Population Membership could not be realized.");

            var result = await _resolverRegistry.Fetch(context.Population.ResolverKey!).Resolve(
                request.SessionId,
                context.Population,
                request.Value.OrganizationId!.Value,
                request.Value.ActivationScopeId);

            return await PopulationReconcile.Execute(
                Connection,
                request.SessionId,
                context.MembershipId.Value,
                result);
        });
    }

    public async Task<Response<IList<Population>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Population>>(request, async response =>
        {
            var populations = await PopulationSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return populations;
        });
    }

    public async Task<Response<Population?>> Fetch(Request<Population> request)
    {
        return await wrappers.Try<Population?>(request, async response =>
        {
            var population = await PopulationSelect.Execute(Connection, request.SessionId, request.Value);

            return population;
        });
    }

    public async Task<Response<IList<PopulationToken>>> FetchTokens(Request<Population> request)
    {
        return await wrappers.Try<IList<PopulationToken>>(request, async response =>
        {
            var population = await PopulationSelect.Execute(Connection, request.SessionId, request.Value);

            return population?.ResolverKey.HasSomething() == true
                ? _resolverRegistry.Fetch(population.ResolverKey!).Tokens
                : [];
        });
    }

}