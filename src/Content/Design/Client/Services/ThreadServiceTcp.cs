using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Client.Services;

public class ThreadServiceTcp(IProxyWrappers proxyWrappers) : IThreadService
{
    public async Task<Response<IList<Thread>>> SearchForForum(Request<ThreadSearch> request) =>
        await proxyWrappers.Send<IList<Thread>>("ThreadSearchForForum", request);

    public async Task<Response<Thread?>> Fetch(Request<Thread> request) =>
        await proxyWrappers.Send<Thread?>("ThreadFetch", request);

    public async Task<Response<Thread?>> Add(Request<Thread> request) =>
        await proxyWrappers.Send<Thread?>("ThreadAdd", request);

    public async Task<Response> Save(Request<Thread> request) =>
        await proxyWrappers.Send("ThreadSave", request);

    public async Task<Response> Remove(Request<Thread> request) =>
        await proxyWrappers.Send("ThreadRemove", request);
}