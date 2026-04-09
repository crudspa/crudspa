using Microsoft.JSInterop;

namespace Crudspa.Framework.Core.Client.Components;

public partial class ConnectingLine : IAsyncDisposable
{
    [Parameter] public String? SourceId { get; set; }
    [Parameter] public String? TargetId { get; set; }

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] public ILogger<ConnectingLine> Logger { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    private DotNetObjectReference<ConnectingLine> _dotNetObjectReference = null!;

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (firstRender)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            await JsBridge.AddResizedListener($"{InstanceId:D}", _dotNetObjectReference);
        }

        await JsBridge.DrawLine(SourceId!, TargetId!);
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetObjectReference.Dispose();
        await JsBridge.RemoveResizedListener($"{InstanceId:D}");
    }

    [JSInvokable]
    public void HandleWindowResized()
    {
        try
        {
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in javascript-invokable method.");
        }
    }
}