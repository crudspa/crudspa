namespace Crudspa.Content.Messaging.Shared.Contracts.Config;

public class PopulationRefreshJobConfig : IValidates
{
    public Guid? PopulationId { get; set; }
    public IList<Guid> OrganizationIds { get; set; } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PopulationId.HasValue)
                errors.AddError("Population is required.", nameof(PopulationId));
            if (OrganizationIds.Count == 0)
                errors.AddError("At least one Organization is required.", nameof(OrganizationIds));
        });
    }
}