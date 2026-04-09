namespace Crudspa.Education.School.Shared.Contracts.Data;

public class ClassRecording : Observable, IValidates
{
    public Guid? Id
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

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!CategoryId.HasValue)
                errors.AddError("Category is required.", nameof(CategoryId));
        });
    }
}