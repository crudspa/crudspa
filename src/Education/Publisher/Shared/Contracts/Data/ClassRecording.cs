namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ClassRecording : Observable, IValidates, ICountable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Uploaded
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile AudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? CategoryId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CategoryName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Unit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Lesson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TeacherNotes
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SchoolContact SchoolContact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? OrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(SchoolContact.Validate());
        });
    }
}