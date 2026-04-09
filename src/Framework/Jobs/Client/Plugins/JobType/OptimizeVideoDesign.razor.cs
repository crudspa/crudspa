using Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

namespace Crudspa.Framework.Jobs.Client.Plugins.JobType;

public partial class OptimizeVideoDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public OptimizeVideoConfig? Config { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<OptimizeVideoConfig>() ?? new();

        await base.OnInitializedAsync();
    }

    public String Description => "All";

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        if (Config is null)
            return [new() { Message = "Config is null." }];

        return Config.Validate();
    }
}