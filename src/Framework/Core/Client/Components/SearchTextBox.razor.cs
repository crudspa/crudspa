using Timer = System.Timers.Timer;

namespace Crudspa.Framework.Core.Client.Components;

public partial class SearchTextBox : IDisposable
{
    [Parameter] public String? Value { get; set; }
    [Parameter] public String? Placeholder { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Int32? MaxLength { get; set; }
    [Parameter] public Int32 Debounce { get; set; } = 250;

    public String? TextValue { get; private set; }

    private Timer? _timer;

    protected override void OnParametersSet()
    {
        TextValue = Value;
    }

    private void OnInput(ChangeEventArgs args)
    {
        TextValue = (String?)args.Value;

        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        _timer = new(Debounce) { AutoReset = false };
        _timer.Elapsed += async (_, _) => await InvokeAsync(() => ValueChanged.InvokeAsync(TextValue));
        _timer.Start();
    }

    public void Dispose()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}