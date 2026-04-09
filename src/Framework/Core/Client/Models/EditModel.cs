namespace Crudspa.Framework.Core.Client.Models;

public class EditModel<T>(Boolean isNew) : ScreenModel
    where T : IObservable
{
    public T? Entity
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsNew
    {
        get => isNew;
        set => SetProperty(ref isNew, value);
    }

    public Boolean ReadOnly
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public void Edit()
    {
        ReadOnly = false;
    }
}