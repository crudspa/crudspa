namespace Crudspa.Framework.Core.Client.Components;

public partial class ErrorRecover
{
    [Inject] public ILogger<ErrorRecover> Logger { get; set; } = null!;

    private Boolean ShowError { get; set; }

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "ErrorRecover caught an unhandled exception. {message} {stackTrace}", exception.Message, exception.StackTrace);

        ShowError = true;

        return InvokeAsync(StateHasChanged);
    }
}