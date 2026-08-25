namespace Crudspa.Content.Messaging.Client.Services;

public class MessageServiceTcp(IProxyWrappers proxyWrappers) : IMessageService
{
    public async Task<Response<IList<Message>>> SearchForMembership(Request<MessageSearch> request) =>
        await proxyWrappers.Send<IList<Message>>("MessageSearchForMembership", request);

    public async Task<Response<IList<Message>>> FetchForActivation(Request<Activation> request) =>
        await proxyWrappers.Send<IList<Message>>("MessageFetchForActivation", request);
    public async Task<Response<IList<Message>>> FetchForStage(Request<Stage> request) => await proxyWrappers.Send<IList<Message>>("MessageFetchForStage",request);
    public async Task<Response<Message?>> Fetch(Request<Message> request) => await proxyWrappers.Send<Message?>("MessageFetch",request);
    public async Task<Response<Message?>> Add(Request<Message> request) => await proxyWrappers.Send<Message?>("MessageAdd",request);
    public async Task<Response> Save(Request<Message> request) => await proxyWrappers.Send("MessageSave",request);
    public async Task<Response> Remove(Request<Message> request) => await proxyWrappers.Send("MessageRemove",request);
}