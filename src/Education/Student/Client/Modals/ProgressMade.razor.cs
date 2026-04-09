using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Modals;

public partial class ProgressMade : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ProgressMadeModel Model { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ProgressMadeModel(IScrollService scrollService) : ModalModel(scrollService)
{
    public MadeProgress? MadeProgress
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override Task Hide()
    {
        MadeProgress = null;
        return base.Hide();
    }
}