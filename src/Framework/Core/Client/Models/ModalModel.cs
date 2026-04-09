namespace Crudspa.Framework.Core.Client.Models;

public class ModalModel(IScrollService scrollService) : ScreenModel
{
    public Guid Id { get; } = Guid.NewGuid();

    public Boolean Visible
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public virtual Task Hide()
    {
        Visible = false;
        Alerts.Clear();
        return Task.CompletedTask;
    }

    public virtual async Task Show()
    {
        Visible = true;
        await scrollService.ToId(Id);
    }
}