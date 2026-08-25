using PermissionIds = Crudspa.Content.Messaging.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<Message>>> MessageSearchForMembership(Request<MessageSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await MessageService.SearchForMembership(request);
        });
    }

    public async Task<Response<IList<Message>>> MessageFetchForActivation(Request<Activation> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await MessageService.FetchForActivation(request));
    }

    public async Task<Response<IList<Message>>> MessageFetchForStage(Request<Stage> request) => await HubWrappers.RequirePermission(request,PermissionIds.Campaigns,async session=>await MessageService.FetchForStage(request));
    public async Task<Response<Message?>> MessageFetch(Request<Message> request) => await HubWrappers.RequirePermission(request,PermissionIds.Campaigns,async session=>await MessageService.Fetch(request));
    public async Task<Response<Message?>> MessageAdd(Request<Message> request) => await HubWrappers.RequirePermission(request,PermissionIds.Campaigns,async session=>{var response=await MessageService.Add(request);if(response.Ok)await Notify(request.SessionId,PermissionIds.Campaigns,new MessageAdded{Id=response.Value.Id,StageId=request.Value.StageId});return response;});
    public async Task<Response> MessageSave(Request<Message> request) => await HubWrappers.RequirePermission(request,PermissionIds.Campaigns,async session=>{var response=await MessageService.Save(request);if(response.Ok)await Notify(request.SessionId,PermissionIds.Campaigns,new MessageSaved{Id=request.Value.Id,StageId=request.Value.StageId});return response;});
    public async Task<Response> MessageRemove(Request<Message> request) => await HubWrappers.RequirePermission(request,PermissionIds.Campaigns,async session=>{var response=await MessageService.Remove(request);if(response.Ok)await Notify(request.SessionId,PermissionIds.Campaigns,new MessageRemoved{Id=request.Value.Id,StageId=request.Value.StageId});return response;});
}