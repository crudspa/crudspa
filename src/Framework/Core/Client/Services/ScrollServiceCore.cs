namespace Crudspa.Framework.Core.Client.Services;

public class ScrollServiceCore(IJsBridge jsBridge) : IScrollService
{
    public async Task ToTop()
    {
        await jsBridge.ScrollToTop();
    }

    public async Task ToBottom()
    {
        await jsBridge.ScrollToBottom();
    }

    public async Task ToId(String id)
    {
        await Task.Delay(100);
        await jsBridge.ScrollToId(id);
    }

    public async Task ToId(Guid id)
    {
        await ToId(id.ToString("D"));
    }
}