namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class Assessment : Observable, IValidates, INamed, ICountable
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

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? AvailableStart
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? AvailableEnd
    {
        get;
        set => SetProperty(ref field, value);
    }

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

    public ImageFile ImageFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? VocabPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ListenPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadPartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenPart> ListenParts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ReadPart> ReadParts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<VocabPart> VocabParts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public AssessmentAssignment? Assignment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 100)
                errors.AddError("Name cannot be longer than 100 characters.", nameof(Name));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!AvailableStart.HasValue)
                errors.AddError("Available Start is required.", nameof(AvailableStart));

            if (!AvailableEnd.HasValue)
                errors.AddError("Available End is required.", nameof(AvailableEnd));

            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));
        });
    }
}