using Crudspa.Content.Messaging.Shared.Contracts.Config;

namespace Crudspa.Content.Jobs.Client.Plugins.JobType;

public partial class RefreshPopulationsDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public RefreshPopulationsModel? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = new(ConfigJson.FromJson<PopulationRefreshJobConfig>() ?? new());
        await base.OnInitializedAsync();
    }

    public String Description => Model?.Description ?? "Refresh Populations";

    public String? GetConfigJson() => Model?.Config.ToJson();

    public List<Error> Validate() => Model?.Validate() ?? [new() { Message = "Config is null." }];
}

public class RefreshPopulationsModel(PopulationRefreshJobConfig config)
{
    private String? _populationId;
    private String? _organizationIds;
    private IList<String> _invalidOrganizationIds = [];

    public PopulationRefreshJobConfig Config { get; } = config;

    public String? PopulationId
    {
        get => _populationId ?? Config.PopulationId?.ToString("D");
        set
        {
            _populationId = value;
            Config.PopulationId = Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public String? OrganizationIds
    {
        get => _organizationIds ?? String.Join(Environment.NewLine, Config.OrganizationIds.Select(x => x.ToString("D")));
        set
        {
            _organizationIds = value;
            var values = value?.Split(['\r', '\n', ',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
            _invalidOrganizationIds = values.Where(x => !Guid.TryParse(x, out _)).ToList();
            Config.OrganizationIds = values.Where(x => Guid.TryParse(x, out _)).Select(Guid.Parse).Distinct().ToList();
        }
    }

    public String Description => Config.PopulationId.HasValue
        ? $"{Config.OrganizationIds.Count:N0} Organizations"
        : "Refresh Populations";

    public List<Error> Validate()
    {
        var errors = Config.Validate();

        if (_populationId.HasSomething() && !Guid.TryParse(_populationId, out _))
            errors.AddError("Population ID must be a GUID.", nameof(PopulationId));

        if (_invalidOrganizationIds.HasItems())
            errors.AddError($"Invalid Organization IDs: {String.Join(", ", _invalidOrganizationIds)}.", nameof(OrganizationIds));

        return errors;
    }
}