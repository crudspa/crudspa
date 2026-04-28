using Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

namespace Crudspa.Framework.Jobs.Client.Plugins.JobType;

public partial class KeepAliveDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public KeepAliveConfig? Config { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<KeepAliveConfig>() ?? new();

        await base.OnInitializedAsync();
    }

    public String Description
    {
        get
        {
            var count = Config?.GetUrls().Count ?? 0;
            return count == 1 ? "1 URL" : $"{count:N0} URLs";
        }
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        if (Config is null)
            return [new() { Message = "Config is null." }];

        return Config.Validate();
    }
}