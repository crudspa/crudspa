using Microsoft.JSInterop;

namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IJsBridge
{
    Task<String> GetCookie(String key, String defaultValue = "");
    Task SetCookie(String key, String value, DateTimeOffset? expires = null);
    Task RefreshBrowser(Int32 secondsToWait = 0);
    Task CopyToClipboard(String text);
    Task<String?> ReadFromClipboard();
    Task ScrollToTop();
    Task ScrollToBottom();
    Task ScrollToId(String id);
    Task PlaySound(String key);
    Task AddAudioListeners<T>(String id, DotNetObjectReference<T> reference) where T : class;
    Task PlayAudio(String id);
    Task StopAudio(String id);
    Task InitializeVideo<T>(String id, DotNetObjectReference<T> reference) where T : class;
    Task DisposeVideo(String id);
    Task PlayVideo(String id);
    Task StopVideo(String id);
    Task AddResizedListener<T>(String id, DotNetObjectReference<T> reference) where T : class;
    Task RemoveResizedListener(String id);
    Task DrawLine(String sourceId, String targetId);
    Task InitializeLinkInterceptor<T>(String id, DotNetObjectReference<T> reference) where T : class;
    Task DisposeLinkInterceptor(String id);
    Task RemoveAudioListeners(String id);
    Task<String?> GetObjectUrlFromInput(String inputId);
    Task RevokeObjectUrl(String? url);
}