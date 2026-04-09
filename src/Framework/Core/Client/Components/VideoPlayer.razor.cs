using Microsoft.JSInterop;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class VideoPlayer : IAsyncDisposable, IHandle<SilenceRequested>
{
    [Parameter, EditorRequired] public VideoFile? VideoFile { get; set; }
    [Parameter] public EventCallback<MediaPlay> MediaPlayed { get; set; }
    [Parameter] public Boolean? AutoPlay { get; set; } = false;
    [Parameter] public ImageFile? Poster { get; set; }
    [Parameter] public Int32? Width { get; set; }
    [Parameter] public String CssClass { get; set; } = String.Empty;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] public ILogger<VideoPlayer> Logger { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    public String Id => $"{InstanceId:D}-{VideoFile!.Id:D}";
    public String DivStyles { get; private set; } = String.Empty;
    public String AspectRatioStyle { get; private set; } = "16/9";
    public IReadOnlyDictionary<String, Object> VideoAttributes => _videoAttributes;

    public String PosterUrl => Poster?.Id is not null ? Poster.FetchUrl() : VideoFile.FetchPosterUrl();
    public String PreloadMode => AutoPlay == true ? "auto" : "none";
    public String VideoUrl => VideoFile.FetchUrl();

    private DotNetObjectReference<VideoPlayer>? _dotNetObjectReference;
    private readonly MediaPlay _mediaPlay = new();
    private readonly Dictionary<String, Object> _videoAttributes = new();
    private String? _priorId;
    private String? _wiredElementId;
    private Int64 _lastPlayRequested;
    private Boolean _autoPlayPending;

    private Boolean HasVideo => VideoFile?.Id is not null;

    protected override void OnParametersSet()
    {
        if (!HasVideo)
            return;

        _videoAttributes.Clear();

        var (intrinsicWidth, intrinsicHeight) = TryGetDimensions();

        if (intrinsicWidth is { } videoIntrinsicWidth && intrinsicHeight is { } videoIntrinsicHeight)
        {
            _videoAttributes["width"] = videoIntrinsicWidth;
            _videoAttributes["height"] = videoIntrinsicHeight;
            AspectRatioStyle = $"{videoIntrinsicWidth}/{videoIntrinsicHeight}";
        }
        else
            AspectRatioStyle = "16/9";

        if (PosterUrl.HasSomething())
            _videoAttributes["poster"] = PosterUrl;

        var wrapperStyles = new G.List<String>
        {
            "min-width:0; min-height:0; box-sizing:border-box; overflow:hidden;",
            $"aspect-ratio:{AspectRatioStyle};",
            "margin-left:auto; margin-right:auto;",
            "max-width:100%;",
        };

        if (Width is { } fixedWidth)
            wrapperStyles.Add($"width:{fixedWidth:D}px;");
        else
            wrapperStyles.Add("width:100%;");

        DivStyles = String.Concat(wrapperStyles);

        if (_mediaPlay.VideoFileId != VideoFile?.Id)
        {
            _priorId = _wiredElementId;
            _mediaPlay.VideoFileId = VideoFile?.Id;
            _mediaPlay.Started = null;
            _mediaPlay.Canceled = null;
            _mediaPlay.Completed = null;
            _lastPlayRequested = 0;
            _autoPlayPending = AutoPlay == true;
            Playing = false;
            _wiredElementId = null;
        }

        if (AutoPlay == true && _mediaPlay.Started is null && _mediaPlay.Canceled is null && _mediaPlay.Completed is null && !Playing)
            _autoPlayPending = true;
    }

    private (Int32? Width, Int32? Height) TryGetDimensions()
    {
        if (VideoFile?.Width is { } videoWidth && videoWidth > 0
            && VideoFile?.Height is { } videoHeight && videoHeight > 0)
            return (videoWidth, videoHeight);

        if (Poster?.Width is { } posterWidth && posterWidth > 0
            && Poster?.Height is { } posterHeight && posterHeight > 0)
            return (posterWidth, posterHeight);

        return (null, null);
    }

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (!HasVideo)
            return;

        _dotNetObjectReference ??= DotNetObjectReference.Create(this);

        if (_priorId.HasSomething())
        {
            await JsBridge.DisposeVideo(_priorId!);
            _priorId = null;
        }

        if (_wiredElementId is null || !_wiredElementId.IsBasically(Id))
        {
            await JsBridge.InitializeVideo(Id, _dotNetObjectReference);
            _wiredElementId = Id;
        }

        if (_autoPlayPending && AutoPlay == true && HasVideo)
        {
            _autoPlayPending = false;
            await Play();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override Task OnInitializedAsync()
    {
        EventBus.Subscribe(this);

        _mediaPlay.VideoFileId = VideoFile?.Id;
        _autoPlayPending = AutoPlay == true;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            EventBus.Unsubscribe(this);

            if (_wiredElementId.HasSomething())
                await JsBridge.DisposeVideo(_wiredElementId!);

            _dotNetObjectReference?.Dispose();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in event handler.");
        }
    }

    public async Task Handle(SilenceRequested payload)
    {
        if (payload.ExceptInstanceId.HasValue && payload.ExceptInstanceId.Value == InstanceId)
            return;

        if (payload.Number <= _lastPlayRequested)
            return;

        await Stop();
    }

    public Boolean Playing
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            _ = InvokeAsync(StateHasChanged);
        }
    }

    public async Task Play()
    {
        if (!HasVideo)
            return;

        _mediaPlay.Started = null;
        _mediaPlay.Canceled = null;
        _mediaPlay.Completed = null;

        var silence = new SilenceRequested { ExceptInstanceId = InstanceId };
        _lastPlayRequested = silence.Number;

        await EventBus.Publish(silence);
        await JsBridge.PlayVideo(Id);
    }

    public async Task Stop()
    {
        if (!HasVideo) return;

        await JsBridge.StopVideo(Id);

        if (_mediaPlay.Started is null && !Playing)
            return;

        await FinalizePlay(false);
    }

    [JSInvokable]
    public async Task HandleVideoEnded()
    {
        try
        {
            await FinalizePlay(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in javascript-invokable method.");
        }
    }

    [JSInvokable]
    public async Task HandleVideoPlayed()
    {
        try
        {
            if (!HasVideo) return;

            Playing = true;

            await EventBus.Publish(new SilenceRequested { ExceptInstanceId = InstanceId });

            _mediaPlay.Started ??= DateTimeOffset.Now;
            _mediaPlay.Canceled = _mediaPlay.Completed = null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in javascript-invokable method.");
        }
    }

    [JSInvokable]
    public async Task HandleVideoStopped()
    {
        try
        {
            await FinalizePlay(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in javascript-invokable method.");
        }
    }

    [JSInvokable]
    public async Task HandleVideoError(String? message)
    {
        try
        {
            Logger.LogError("Video error. VideoFileId={VideoFileId}. {Message}", VideoFile?.Id, message ?? "No message.");
            await FinalizePlay(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in javascript-invokable method.");
        }
    }

    private async Task FinalizePlay(Boolean completed)
    {
        if (_mediaPlay.Started is null && !Playing)
            return;

        if (_mediaPlay.Completed is not null || _mediaPlay.Canceled is not null)
            return;

        if (completed)
            _mediaPlay.Completed = DateTimeOffset.Now;
        else
            _mediaPlay.Canceled = DateTimeOffset.Now;

        Playing = false;

        if (MediaPlayed.HasDelegate)
            await MediaPlayed.InvokeAsync(_mediaPlay);
    }
}