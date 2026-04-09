using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IThreadService
{
    Task<Response<IList<Thread>>> SearchForForum(Request<ThreadSearch> request);
    Task<Response<Thread?>> Fetch(Request<Thread> request);
    Task<Response<Thread?>> Add(Request<Thread> request);
    Task<Response> Save(Request<Thread> request);
    Task<Response> Remove(Request<Thread> request);
}