namespace Crudspa.Content.Messaging.Server.Services;

public class SmsPreferenceServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISmsPreferenceService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SmsPreference>>> SearchForPortal(Request<SmsPreferenceSearch> request)
    {
        return await wrappers.Try<IList<SmsPreference>>(request, async response =>
        {
            var smsPreferences = await SmsPreferenceSelectWhereForPortal.Execute(Connection, request.SessionId, request.Value);

            return smsPreferences;
        });
    }

    public async Task<Response<SmsPreference?>> Fetch(Request<SmsPreference> request)
    {
        return await wrappers.Try<SmsPreference?>(request, async response =>
        {
            var smsPreference = await SmsPreferenceSelect.Execute(Connection, request.SessionId, request.Value);

            return smsPreference;
        });
    }

    public async Task<Response<SmsPreference?>> Add(Request<SmsPreference> request)
    {
        return await wrappers.Validate<SmsPreference?, SmsPreference>(request, async response =>
        {
            var smsPreference = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SmsPreferenceInsert.Execute(connection, transaction, request.SessionId, smsPreference);

                return new SmsPreference
                {
                    Id = id,
                    PortalId = smsPreference.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SmsPreference> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var smsPreference = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsPreferenceUpdate.Execute(connection, transaction, request.SessionId, smsPreference);
            });
        });
    }

    public async Task<Response> Remove(Request<SmsPreference> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var smsPreference = request.Value;
            var existing = await SmsPreferenceSelect.Execute(Connection, request.SessionId, smsPreference);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsPreferenceDelete.Execute(connection, transaction, request.SessionId, smsPreference);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchContactNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await Crudspa.Framework.Core.Server.Sproxies.ContactSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchContactPhoneNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Framework.Core.Server.Sproxies.ContactPhoneSelectOrderables.Execute(Connection, request.SessionId));
    }
}