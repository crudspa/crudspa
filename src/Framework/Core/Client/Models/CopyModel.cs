namespace Crudspa.Framework.Core.Client.Models;

public class CopyModel(IScrollService scrollService) : ModalModel(scrollService)
{
    public Copy Copy
    {
        get;
        set => SetProperty(ref field, value);
    } = new();
}