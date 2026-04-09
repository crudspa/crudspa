using Microsoft.AspNetCore.Components.Web;

namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IClickService
{
    event EventHandler<MouseEventArgs> Clicked;
    void Click(MouseEventArgs args);
}