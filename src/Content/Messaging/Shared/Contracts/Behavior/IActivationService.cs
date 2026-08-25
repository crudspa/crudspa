namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IActivationService
{
    Task<Response<IList<ActivationTarget>>> SearchTargets(Request<ActivationTargetSearch> request);
    Task<Response<IList<CampaignScheduleOption>>> FetchScheduleOptions(Request<ActivationTarget> request);
    Task<Response<IList<Activation>>> SearchForOrganization(Request<ActivationSearch> request);
    Task<Response<IList<Activation>>> FetchForCampaign(Request<Campaign> request);
    Task<Response<CampaignActivationResult?>> Activate(Request<CampaignActivation> request);
    Task<Response<CampaignScheduleConfiguration?>> FetchSchedule(Request<Activation> request);
    Task<Response> SaveSchedule(Request<CampaignScheduleConfiguration> request);

    Task<Response<Activation?>> Fetch(Request<Activation> request) =>
        Task.FromResult<Response<Activation?>>(new() { Errors = [new() { Message = "Activation detail is provided by the activation workflow." }] });

    Task<Response> Remove(Request<Activation> request) =>
        Task.FromResult<Response>(new() { Errors = [new() { Message = "Activation removal is not available." }] });
}