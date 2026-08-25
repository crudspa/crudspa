namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IMessageService
{
    Task<Response<IList<Message>>> SearchForMembership(Request<MessageSearch> request);
    Task<Response<IList<Message>>> FetchForActivation(Request<Activation> request);
    Task<Response<IList<Message>>> FetchForStage(Request<Stage> request);
    Task<Response<Message?>> Fetch(Request<Message> request);
    Task<Response<Message?>> Add(Request<Message> request);
    Task<Response> Save(Request<Message> request);
    Task<Response> Remove(Request<Message> request);
}