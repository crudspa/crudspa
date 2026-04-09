namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class CoursePage : Observable
{
    public Guid? CourseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IList<Page> Pages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}