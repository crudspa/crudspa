namespace Crudspa.Content.Design.Server.Services;

public class SmsTemplateServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISmsTemplateService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SmsTemplate>>> SearchForMembership(Request<SmsTemplateSearch> request)
    {
        return await wrappers.Try<IList<SmsTemplate>>(request, async response =>
        {
            var smsTemplates = await SmsTemplateSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return smsTemplates;
        });
    }

    public async Task<Response<IList<SmsTemplate>>> SearchForPortal(Request<SmsTemplateSearch> request)
    {
        return await wrappers.Try<IList<SmsTemplate>>(request, async response =>
        {
            var smsTemplates = await SmsTemplateSelectWhereForPortal.Execute(Connection, request.SessionId, request.Value);

            return smsTemplates;
        });
    }

    public async Task<Response<SmsTemplate?>> Fetch(Request<SmsTemplate> request)
    {
        return await wrappers.Try<SmsTemplate?>(request, async response =>
        {
            var smsTemplate = await SmsTemplateSelect.Execute(Connection, request.SessionId, request.Value);

            return smsTemplate;
        });
    }

    public async Task<Response<SmsTemplate?>> Add(Request<SmsTemplate> request)
    {
        return await wrappers.Validate<SmsTemplate?, SmsTemplate>(request, async response =>
        {
            var smsTemplate = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SmsTemplateInsert.Execute(connection, transaction, request.SessionId, smsTemplate);

                return new SmsTemplate
                {
                    Id = id,
                    MembershipId = smsTemplate.MembershipId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SmsTemplate> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var smsTemplate = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsTemplateUpdate.Execute(connection, transaction, request.SessionId, smsTemplate);
            });
        });
    }

    public async Task<Response> Remove(Request<SmsTemplate> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var smsTemplate = request.Value;
            var existing = await SmsTemplateSelect.Execute(Connection, request.SessionId, smsTemplate);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsTemplateDelete.Execute(connection, transaction, request.SessionId, smsTemplate);
            });
        });
    }
}