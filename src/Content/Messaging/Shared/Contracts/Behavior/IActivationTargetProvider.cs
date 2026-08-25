namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IActivationTargetProvider
{
    Guid PortalId { get; }
    Task<IList<ActivationTarget>> Search(Guid? sessionId, Guid? campaignId, String? text);
    Task<Boolean> Validate(Guid? sessionId, Guid? campaignId, Guid organizationId);
}

public class ActivationTarget
{
    public Guid OrganizationId { get; set; }
    public String Name { get; set; } = String.Empty;
    public String? Kind { get; set; }
    public Guid? ParentOrganizationId { get; set; }
    public String? ParentName { get; set; }
}