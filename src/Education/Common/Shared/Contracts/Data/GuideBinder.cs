namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class GuideBinder : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile GuideImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<GuidePage> Pages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}