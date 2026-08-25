namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class ActivationTargetSearch : IValidates
{
    public Guid? PortalId { get; set; }
    public Guid? CampaignId { get; set; }
    public String? Text { get; set; }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PortalId.HasValue)
                errors.AddError("Portal is required.", nameof(PortalId));

            if (!CampaignId.HasValue)
                errors.AddError("Campaign is required.", nameof(CampaignId));

            if (Text?.Length > 100)
                errors.AddError("Search cannot be longer than 100 characters.", nameof(Text));
        });
    }
}