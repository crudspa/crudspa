using Crudspa.Content.Jobs.Shared.Contracts.Config;

namespace Crudspa.Content.Jobs.Client.Plugins.JobType;

public partial class SendEmailsDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public SendEmailsConfig? Config { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<SendEmailsConfig>() ?? new();

        await base.OnInitializedAsync();
    }

    public String Description => "Send";

    public String? GetConfigJson() => Config?.ToJson();

    public List<Error> Validate()
    {
        if (Config is null)
            return [new() { Message = "Config is null." }];

        return Config.Validate();
    }
}