using Microsoft.JSInterop;

namespace Crudspa.Framework.Core.Client.Components;

public partial class AudioPlayer : IAsyncDisposable, IHandle<SilenceRequested>
{
    [Parameter, EditorRequired] public AudioFile? AudioFile { get; set; }
    [Parameter] public Boolean Hidden { get; set; }
    [Parameter] public Boolean Compact { get; set; }
    [Parameter] public String? PlayText { get; set; } = "Play";
    [Parameter] public String? StopText { get; set; } = "Stop";
    [Parameter] public String PlayIcon { get; set; } = "c-icon-play-circle";
    [Parameter] public String StopIcon { get; set; } = "c-icon-stop-circle";
    [Parameter] public EventCallback<MediaPlay> MediaPlayed { get; set; }
    [Parameter] public Boolean AutoPlay { get; set; }
    [Parameter] public Controls Control { get; set; } = Controls.Button;
    [Parameter] public Boolean Disabled { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] public ILogger<AudioPlayer> Logger { get; set; } = null!;
    [Inject] public NavigationManager Navigation { get; set; } = null!;

    public enum Controls { Button, Native }

    public readonly Guid InstanceId = Guid.NewGuid();

    private readonly MediaPlay _mediaPlay = new();
    private DotNetObjectReference<AudioPlayer>? _dotNetObjectReference;

    private String ElementId => $"{InstanceId:D}-{AudioFile!.Id:D}";
    private Boolean ShowNative => Control == Controls.Native;

    private String SourceUrl => AudioFile.FetchUrl();

    private String? _wiredElementId;

    private Boolean HasAudio => AudioFile?.Id is not null;
    private Boolean Unplayable { get; set; }
    private Boolean IsPlayable => !Disabled && !Unplayable && HasAudio;

    private Int32 _sessionToken;
    private Int32 _activeToken;
    private Int64 _lastPlayRequested;
    private Boolean _autoPlayPending;

    protected override Task OnInitializedAsync()
    {
        EventBus.Subscribe(this);
        _mediaPlay.AudioFileId = AudioFile?.Id;
        _autoPlayPending = AutoPlay;
        return Task.CompletedTask;
    }

    protected override Task OnParametersSetAsync()
    {
        var newId = AudioFile?.Id;
        var newElementId = newId is null ? null : $"{InstanceId:D}-{newId:D}";

        if (!String.Equals(_wiredElementId, newElementId, StringComparison.Ordinal))
        {
            if (_wiredElementId is not null)
                _ = JsBridge.StopAudio(_wiredElementId);

            _wiredElementId = null;

            _mediaPlay.AudioFileId = newId;
            _mediaPlay.Started = null;
            _mediaPlay.Canceled = null;
            _mediaPlay.Completed = null;
            _lastPlayRequested = 0;
            _autoPlayPending = AutoPlay;

            Unplayable = false;
        }

        if (Disabled && Playing) _ = Stop();

        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (!HasAudio) return;

        _dotNetObjectReference ??= DotNetObjectReference.Create(this);

        if (_wiredElementId is null || !_wiredElementId.IsBasically(ElementId))
        {
            if (_wiredElementId is not null)
                await JsBridge.RemoveAudioListeners(_wiredElementId);

            await JsBridge.AddAudioListeners(ElementId, _dotNetObjectReference);
            _wiredElementId = ElementId;
        }

        if (_autoPlayPending && IsPlayable && _wiredElementId is not null && _wiredElementId.IsBasically(ElementId))
        {
            _autoPlayPending = false;
            await Play();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            EventBus.Unsubscribe(this);

            if (_wiredElementId is not null)
                await JsBridge.RemoveAudioListeners(_wiredElementId);

            try
            {
                await JsBridge.StopAudio(_wiredElementId ?? ElementId);
            }
            finally
            {
                _dotNetObjectReference?.Dispose();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in DisposeAsync.");
        }
    }

    public async Task Handle(SilenceRequested payload)
    {
        if (payload.ExceptInstanceId.HasValue && payload.ExceptInstanceId.Value.Equals(InstanceId))
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
        if (!IsPlayable) return;

        _sessionToken++;
        _activeToken = _sessionToken;

        _mediaPlay.Started = null;
        _mediaPlay.Canceled = null;
        _mediaPlay.Completed = null;

        var silence = new SilenceRequested { ExceptInstanceId = InstanceId };
        _lastPlayRequested = silence.Number;

        await EventBus.Publish(silence);
        await JsBridge.PlayAudio(ElementId);
    }

    public async Task PlayAndWait()
    {
        if (!IsPlayable) return;

        await Play();

        var token = _activeToken;
        var timeoutAt = DateTimeOffset.Now.AddSeconds(15);

        while (DateTimeOffset.Now < timeoutAt
               && token == _activeToken
               && _mediaPlay.Completed is null
               && _mediaPlay.Canceled is null)
            await Task.Delay(100);
    }

    public async Task Stop()
    {
        if (!HasAudio) return;
        await JsBridge.StopAudio(_wiredElementId ?? ElementId);
    }

    private async Task FinalizePlay(Boolean completed, Double currentTime = 0, Double duration = 0, Int32? token = null)
    {
        if (token.HasValue && token.Value != _activeToken) return;
        if (_mediaPlay.Completed is not null || _mediaPlay.Canceled is not null) return;

        if (completed) _mediaPlay.Completed = DateTimeOffset.Now;
        else _mediaPlay.Canceled = DateTimeOffset.Now;

        Playing = false;

        if (MediaPlayed.HasDelegate) await MediaPlayed.InvokeAsync(_mediaPlay);
    }

    [JSInvokable]
    public async Task HandleAudioPlaying()
    {
        try
        {
            if (!IsPlayable) return;

            await EventBus.Publish(new SilenceRequested { ExceptInstanceId = InstanceId });

            _mediaPlay.Started ??= DateTimeOffset.Now;
            _mediaPlay.Canceled = null;
            _mediaPlay.Completed = null;

            Playing = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in HandleAudioPlaying.");
        }
    }

    [JSInvokable]
    public Task HandleAudioPaused(Double currentTime, Double duration, Int32 token)
    {
        try
        {
            if (!IsPlayable) return Task.CompletedTask;
            _ = FinalizePlay(false, currentTime, duration, token);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in HandleAudioPaused.");
        }

        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task HandleAudioEnded(Int32 token)
    {
        try
        {
            if (!IsPlayable) return;
            await FinalizePlay(true, 0, 0, token);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in HandleAudioEnded.");
        }
    }

    [JSInvokable]
    public void HandleAudioError(Int32 code, String? message)
    {
        try
        {
            Playing = false;

            var isHardFailure = code is 3 or 4;

            if (isHardFailure)
                Unplayable = true;

            if (_mediaPlay.Completed is null && _mediaPlay.Canceled is null)
                _ = FinalizePlay(false, token: _activeToken);

            _ = InvokeAsync(StateHasChanged);

            Logger.LogError("Audio error. Code {Code}. AudioFileId={AudioFileId} HardFailure={HardFailure}. {Message}", code, AudioFile?.Id, isHardFailure, message ?? "No message.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in HandleAudioError.");
        }
    }
}