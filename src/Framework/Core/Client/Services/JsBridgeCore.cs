using Microsoft.JSInterop;

namespace Crudspa.Framework.Core.Client.Services;

public class JsBridgeCore(IJSRuntime jsRuntime, NavigationManager navigationManager, ILogger<JsBridgeCore> logger) : IJsBridge
{
    public async Task<String> GetCookie(String key, String defaultValue = "")
    {
        var cookieValue = await jsRuntime.InvokeAsync<String?>("getCookieValue", key);
        return cookieValue.HasSomething() ? cookieValue! : defaultValue;
    }

    public async Task SetCookie(String key, String value, DateTimeOffset? expires = null)
    {
        var expiration = expires?.ToUniversalTime().ToString("R");
        await jsRuntime.InvokeVoidAsync("setCookieValue", key, value, expiration);
    }

    public async Task RefreshBrowser(Int32 secondsToWait = 0)
    {
        try
        {
            if (secondsToWait > 0)
                await Task.Delay(TimeSpan.FromSeconds(secondsToWait));

            navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in RefreshBrowser(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task CopyToClipboard(String text)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("copyToClipboard", text);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in CopyToClipboard(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in CopyToClipboard(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task<String?> ReadFromClipboard()
    {
        try
        {
            return await jsRuntime.InvokeAsync<String?>("readFromClipboard");
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in ReadFromClipboard(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in ReadFromClipboard(). {message} {stackTrace}", ex.Message, ex.StackTrace);
            return null;
        }
    }

    public async Task ScrollToTop()
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("scrollToTop");
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in ScrollToTop(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in ScrollToTop(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task ScrollToBottom()
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("scrollToBottom");
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in ScrollToBottom(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in ScrollToBottom(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task ScrollToId(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("scrollToId", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in ScrollToId(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in ScrollToId(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task PlaySound(String key)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("playSound", key);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in PlaySound(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in PlaySound(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task AddAudioListeners<T>(String id, DotNetObjectReference<T> reference) where T : class
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("addAudioListeners", id, reference);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in AddAudioListeners(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in AddAudioListeners(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task RemoveAudioListeners(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("removeAudioListeners", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in RemoveAudioListeners(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in RemoveAudioListeners(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task PlayAudio(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("playAudio", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in PlayAudio(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in PlayAudio(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task StopAudio(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("stopAudio", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in StopAudio(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in StopAudio(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task InitializeVideo<T>(String id, DotNetObjectReference<T> reference) where T : class
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("initializeVideo", id, reference);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in InitializeVideo(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in InitializeVideo(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task DisposeVideo(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("disposeVideo", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in DisposeVideo(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in DisposeVideo(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task PlayVideo(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("playVideo", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in PlayVideo(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in PlayVideo(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task StopVideo(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("stopVideo", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in StopVideo(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in StopVideo(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task AddResizedListener<T>(String id, DotNetObjectReference<T> reference) where T : class
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("addResizedListener", id, reference);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in AddResizedListener(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in AddResizedListener(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task RemoveResizedListener(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("removeResizedListener", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in RemoveResizedListener(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in RemoveResizedListener(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task DrawLine(String sourceId, String targetId)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("drawLine", sourceId, targetId);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in DrawLine(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in DrawLine(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task InitializeLinkInterceptor<T>(String id, DotNetObjectReference<T> reference) where T : class
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("initializeLinkInterceptor", id, reference);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in InitializeLinkInterceptor(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in InitializeLinkInterceptor(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task DisposeLinkInterceptor(String id)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("disposeLinkInterceptor", id);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in DisposeLinkInterceptor(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in DisposeLinkInterceptor(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public async Task<String?> GetObjectUrlFromInput(String inputId)
    {
        try
        {
            return await jsRuntime.InvokeAsync<String?>("getObjectUrlFromInput", inputId);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in GetObjectUrlFromInput(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in GetObjectUrlFromInput(). {message} {stackTrace}", ex.Message, ex.StackTrace);
            return null;
        }
    }

    public async Task RevokeObjectUrl(String? url)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("revokeObjectUrl", url);
        }
        catch (JSException jsException)
        {
            logger.LogError(jsException, "JSException raised in RevokeObjectUrl(). {message} {stackTrace}", jsException.Message, jsException.StackTrace);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception raised in RevokeObjectUrl(). {message} {stackTrace}", ex.Message, ex.StackTrace);
        }
    }
}