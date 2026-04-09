using Microsoft.JSInterop;

namespace Crudspa.Framework.Core.Client.Components;

public partial class UserHtml : IAsyncDisposable
{
    [Parameter, EditorRequired] public String Html { get; set; } = String.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public String CssClass { get; set; } = String.Empty;

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] public ILogger<UserHtml> Logger { get; set; } = null!;
    [Inject] public ILinkClickService LinkClickService { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    public String Id => $"{InstanceId:D}";

    private DotNetObjectReference<UserHtml>? _dotNetRef;

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await JsBridge.InitializeLinkInterceptor(Id, _dotNetRef);
        }
    }

    [JSInvokable]
    public void HandleLinkClicked(String url)
    {
        var linkClick = new LinkClick { Url = url };
        _ = LinkClickService.Add(new(linkClick));
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        await JsBridge.DisposeLinkInterceptor(Id);
    }
}