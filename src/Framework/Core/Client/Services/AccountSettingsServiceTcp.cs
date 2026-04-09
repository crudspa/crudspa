namespace Crudspa.Framework.Core.Client.Services;

public class AccountSettingsServiceTcp(IProxyWrappers proxyWrappers) : IAccountSettingsService
{
    public async Task<Response<User?>> Fetch(Request request) =>
        await proxyWrappers.Send<User?>("AccountSettingsFetch", request);

    public async Task<Response> Save(Request<User> request) =>
        await proxyWrappers.Send("AccountSettingsSave", request);
}