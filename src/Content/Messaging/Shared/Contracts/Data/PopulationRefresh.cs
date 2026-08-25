namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class PopulationRefresh : IValidates
{
    public Guid? PopulationId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ActivationScopeId { get; set; }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PopulationId.HasValue)
                errors.AddError("Population is required.", nameof(PopulationId));
            if (!OrganizationId.HasValue)
                errors.AddError("Organization is required.", nameof(OrganizationId));
        });
    }
}

public class PopulationRefreshResult
{
    public Guid? MembershipId { get; set; }
    public Int32 Added { get; set; }
    public Int32 Removed { get; set; }
    public Int32 Preserved { get; set; }
    public Int32 OptedOut { get; set; }
    public Int32 Tokens { get; set; }
    public Int32 TokenValues { get; set; }
}