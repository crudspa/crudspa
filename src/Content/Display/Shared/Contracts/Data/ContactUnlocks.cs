namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ContactUnlocks : Observable
{
    public ObservableCollection<CourseUnlock> Courses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<TrackUnlock> Tracks
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}