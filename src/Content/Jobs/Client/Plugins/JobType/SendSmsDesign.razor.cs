namespace Crudspa.Content.Jobs.Client.Plugins.JobType;

public partial class SendSmsDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public SendSmsConfig? Config { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<SendSmsConfig>() ?? new();

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