namespace Crudspa.Framework.Jobs.Client.Plugins.JobType;

public partial class NoConfigDesign : IJobDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public String Description => "No configuration";

    public String GetConfigJson() => "{}";

    public List<Error> Validate() => [];
}