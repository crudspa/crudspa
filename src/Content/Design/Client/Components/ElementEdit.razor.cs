using Crudspa.Content.Design.Client.Plugins.ElementType;

namespace Crudspa.Content.Design.Client.Components;

public partial class ElementEdit
{
    private const String StylesTab = "Styles";
    private const String SizingTab = "Sizing";

    [Parameter, EditorRequired] public ElementEditModel Model { get; set; } = null!;
    [Parameter, EditorRequired] public List<ElementType> ElementTypes { get; set; } = [];
    [Parameter] public Boolean ReadOnly { get; set; }

    public String ActiveElementSettingsTab { get; private set; } = StylesTab;

    public String SettingsDescription => BuildSettingsDescription(Model.BoxModel.Description, Model.ItemModel.Description);

    public Boolean MultimediaSettingsHandled =>
        Model.SelectedElementType?.EditorView?.Contains(nameof(MultimediaElementDesign), StringComparison.Ordinal) == true;

    private Task OpenElementSettings()
    {
        if (ReadOnly)
            return Task.CompletedTask;

        ActiveElementSettingsTab = StylesTab;
        Model.ItemModel.RefreshOverrideStates();
        return Model.BoxModel.Show();
    }

    private Task HideElementSettings()
    {
        Model.ItemModel.NormalizeOverrideValues();
        return Model.BoxModel.Hide();
    }

    private void SetElementSettingsTab(String tab) => ActiveElementSettingsTab = tab;

    private static String BuildSettingsDescription(params String?[] descriptions)
    {
        var values = descriptions
            .Where(x => x.HasSomething())
            .SelectMany(x => x!.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(x => x.HasSomething() && !x.IsBasically("None"))
            .ToList();

        return values.HasItems() ? String.Join(" | ", values) : "Default";
    }
}