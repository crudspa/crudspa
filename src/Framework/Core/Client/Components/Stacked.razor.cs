namespace Crudspa.Framework.Core.Client.Components;

public partial class Stacked
{
    [Parameter] public Alignments Alignment { get; set; } = Alignments.Default;
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;

    public enum Alignments
    {
        Default, Center, Right, SelfTop,
    }

    public String StackClass
    {
        get
        {
            var stackClass = "c-stack ";

            switch (Alignment)
            {
                case Alignments.Default:
                    break;
                case Alignments.Center:
                    stackClass += "center ";
                    break;
                case Alignments.Right:
                    stackClass += "right ";
                    break;
                case Alignments.SelfTop:
                    stackClass += "self-top ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Alignment));
            }

            return stackClass;
        }
    }
}