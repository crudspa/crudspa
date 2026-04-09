using Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

namespace Crudspa.Framework.Jobs.Client.Plugins.JobType;

public partial class ResetFilesDesign : IJobDesign
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }

    public ResetFilesModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<ResetFilesConfig>() ?? new();

        Model = new(config);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public String Description => Model.Description;

    public String? GetConfigJson()
    {
        return Model.Config.ToJson();
    }

    public List<Error> Validate()
    {
        return Model.Validate();
    }
}

public class ResetFilesModel : Observable, IDisposable
{
    private void HandleConfigChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Config));

    public ResetFilesModel(ResetFilesConfig config)
    {
        Config = config;
        Config.PropertyChanged += HandleConfigChanged;
    }

    public void Dispose()
    {
        Config.PropertyChanged -= HandleConfigChanged;
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public String Description
    {
        get
        {
            var description = String.Empty;

            if (Config.OptimizedAudioFiles == true)
                description += "Audio Optimization | ";

            if (Config.OptimizedImageFiles == true)
                description += "Image Optimization | ";

            if (Config.ImageCaptions == true)
                description += "Image Captions | ";

            if (Config.OptimizedVideoFiles == true)
                description += "Video Optimization | ";

            return description.RemoveLast(" | ");
        }
    }

    public ResetFilesConfig Config { get; }

    public List<Error> Validate() => Config.Validate();
}