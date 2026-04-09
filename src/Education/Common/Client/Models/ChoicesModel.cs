namespace Crudspa.Education.Common.Client.Models;

public class ChoicesModel : Observable
{
    public ObservableCollection<ActivityChoiceModel> Options
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}