using Microsoft.AspNetCore.Components.Web;

namespace Crudspa.Framework.Core.Client.Models;

public class MenuModel(IClickService clickService) : Observable, IDisposable
{
    private Boolean _justOpened;

    public void Dispose()
    {
        clickService.Clicked -= HandleNextClick;
    }

    public Boolean Visible
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void Toggle()
    {
        if (Visible)
        {
            _justOpened = false;
            clickService.Clicked -= HandleNextClick;
            Visible = false;
        }
        else
        {
            _justOpened = true;
            clickService.Clicked += HandleNextClick;
            Visible = true;
        }
    }

    private void HandleNextClick(Object? sender, MouseEventArgs args)
    {
        if (_justOpened)
            _justOpened = false;
        else
        {
            clickService.Clicked -= HandleNextClick;
            Visible = false;
        }
    }
}