using Microsoft.AspNetCore.Components.Web;

namespace Crudspa.Framework.Core.Client.Services;

public class ClickServiceCore : IClickService
{
    public event EventHandler<MouseEventArgs>? Clicked;

    public void Click(MouseEventArgs args)
    {
        var raiseEvent = Clicked;
        raiseEvent?.Invoke(this, args);
    }
}