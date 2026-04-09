using System.Text;
using Crudspa.Content.Display.Client.Extensions;

namespace Crudspa.Content.Display.Client.Components;

public partial class SectionDisplay : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public Section Section { get; set; } = null!;
    [Parameter] public EventCallback<IReadOnlyList<ElementDisplayModel>> ElementDisplayModelsChanged { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public List<ElementDisplayModel> ElementDisplayModels { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        foreach (var element in Section.Elements.OrderBy(x => x.Ordinal))
            ElementDisplayModels.Add(new(EventBus, ElementProgressService, element));

        foreach (var elementModel in ElementDisplayModels)
            elementModel.PropertyChanged += HandleModelChanged;

        if (ElementDisplayModelsChanged.HasDelegate)
            await ElementDisplayModelsChanged.InvokeAsync(ElementDisplayModels);

        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        foreach (var elementModel in ElementDisplayModels)
            elementModel.PropertyChanged -= HandleModelChanged;
    }

    public String SectionStyles
    {
        get
        {
            var styles = new StringBuilder(String.Empty);

            styles.Append(Section.Container.ContainerStyles());
            styles.Append(Section.Box.BoxStyles());

            return styles.ToString();
        }
    }
}