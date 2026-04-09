namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ActivityChoice : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ImageFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsCorrect
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 ColumnOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile? AudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile? ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!IsCorrect.HasValue)
                errors.AddError("Is Correct is required.", nameof(IsCorrect));
        });
    }
}