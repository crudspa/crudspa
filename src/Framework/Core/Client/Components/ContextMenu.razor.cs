namespace Crudspa.Framework.Core.Client.Components;

public partial class ContextMenu : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    public enum ButtonStyles
    {
        None,
        User,
        Transparent,
        Create,
        Edit,
        View,
    }

    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public ButtonStyles ButtonStyle { get; set; } = ButtonStyles.None;
    [Parameter] public Boolean Disabled { get; set; }
    [Parameter] public Boolean HideBorder { get; set; }

    [Inject] public IClickService ClickService { get; set; } = null!;

    public MenuModel Model { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model = new(ClickService);
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public String ButtonClass
    {
        get
        {
            switch (ButtonStyle)
            {
                case ButtonStyles.None:
                    return String.Empty;
                case ButtonStyles.Transparent:
                    return "transparent bordered";
                case ButtonStyles.Create:
                    return "create";
                case ButtonStyles.Edit:
                    return "edit";
                case ButtonStyles.View:
                    return "view";
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }
    }
}