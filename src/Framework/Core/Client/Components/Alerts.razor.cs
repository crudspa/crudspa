using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Components;

public partial class Alerts : IDisposable
{
    private void HandleItemsChanged(Object? sender, NotifyCollectionChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ObservableCollection<Alert> Items { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Items.CollectionChanged += HandleItemsChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Items.CollectionChanged -= HandleItemsChanged;
    }
}