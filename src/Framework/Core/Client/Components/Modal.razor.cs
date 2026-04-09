using System.Text;

namespace Crudspa.Framework.Core.Client.Components;

public partial class Modal : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    public enum ModalScope { Local, Global }

    public enum ModalSizing { Auto, Fluid, Full }

    [Parameter, EditorRequired] public ModalModel? Model { get; set; }
    [Parameter, EditorRequired] public RenderFragment Content { get; set; } = null!;
    [Parameter] public RenderFragment? Buttons { get; set; }
    [Parameter] public String? Title { get; set; }
    [Parameter] public String? Width { get; set; } = "auto";
    [Parameter] public String ZIndex { get; set; } = "300";
    [Parameter] public String TopMargin { get; set; } = ".25em";
    [Parameter] public Boolean ShowOverlay { get; set; } = true;
    [Parameter] public Boolean NoPadding { get; set; }
    [Parameter] public ModalScope Scope { get; set; } = ModalScope.Local;
    [Parameter] public ModalSizing Sizing { get; set; } = ModalSizing.Auto;

    protected override Task OnInitializedAsync()
    {
        if (Model is not null)
        {
            Model.PropertyChanged += HandleModelChanged;
            Model.Title ??= Title;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (Model is not null)
        {
            Model.PropertyChanged -= HandleModelChanged;
            Model.Dispose();
        }
    }

    public String ModalClasses
    {
        get
        {
            var classes = "c-modal";

            if (!ShowOverlay)
                classes += " no-overlay";

            if (Scope == ModalScope.Global)
                classes += " global";

            switch (Sizing)
            {
                case ModalSizing.Fluid:
                    classes += " fluid";
                    break;
                case ModalSizing.Full:
                    classes += " full";
                    break;
                case ModalSizing.Auto:
                default:
                    break;
            }

            return classes;
        }
    }

    public String WindowStyles
    {
        get
        {
            var styles = new StringBuilder();

            if (Scope == ModalScope.Global)
            {
                if (Sizing == ModalSizing.Full)
                {
                    styles.Append("margin:0;");
                    styles.Append("width:100%;");
                    styles.Append("height:100%;");
                    styles.Append("max-height:100vh;");
                }
                else
                {
                    styles.Append($"margin:{TopMargin} auto 2em auto;");
                    styles.Append($"max-height:calc(100vh - ({TopMargin} + 2em));");

                    if (Sizing != ModalSizing.Full && Width.HasSomething())
                        styles.Append($"width:{Width};");
                }

                styles.Append("overflow-y:auto;");
            }
            else
            {
                styles.Append($"margin-top:{TopMargin};");

                if (Sizing != ModalSizing.Full && Width.HasSomething())
                    styles.Append($"width:{Width};");
            }

            return styles.ToString();
        }
    }
}