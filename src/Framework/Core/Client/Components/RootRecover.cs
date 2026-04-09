using Microsoft.AspNetCore.Components.Rendering;

namespace Crudspa.Framework.Core.Client.Components;

public class RootRecover<TApp> : ComponentBase
    where TApp : Microsoft.AspNetCore.Components.IComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<ErrorRecover>(0);
        builder.AddAttribute(1, nameof(ErrorRecover.ChildContent), (RenderFragment)(content =>
        {
            content.OpenComponent(2, typeof(TApp));
            content.CloseComponent();
        }));
        builder.CloseComponent();
    }
}