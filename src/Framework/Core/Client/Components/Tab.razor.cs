namespace Crudspa.Framework.Core.Client.Components;

public partial class Tab
{
    [CascadingParameter] public Tabs? Owner { get; set; }

    [Parameter, EditorRequired] public String? Key { get; set; }
    [Parameter, EditorRequired] public String? Title { get; set; }
    [Parameter] public Boolean Lazy { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        Owner?.Add(new()
        {
            Key = Key,
            Title = Title,
            Lazy = Lazy,
            Content = ChildContent,
        });
    }

    protected override Boolean ShouldRender() => false;
}