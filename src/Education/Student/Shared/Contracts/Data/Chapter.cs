namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Chapter : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PageCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BinderDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ChapterProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!BookId.HasValue)
                errors.AddError("Book is required.", nameof(BookId));

            if (!Ordinal.HasValue)
                errors.AddError("Ordinal is required.", nameof(Ordinal));
        });
    }
}