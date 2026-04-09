namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class StudentAssessment : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentTypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentLevelId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Score
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentLevelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentLevelName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? AssessmentLevelOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? AssessmentTypeKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentTypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? AssessmentTypeOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!StudentId.HasValue)
                errors.AddError("Student is required.", nameof(StudentId));

            if (!AssessmentTypeId.HasValue)
                errors.AddError("Assessment Type is required.", nameof(AssessmentTypeId));

            if (!AssessmentLevelId.HasValue)
                errors.AddError("Level is required.", nameof(AssessmentLevelId));

            if (Score.HasSomething() && Score!.Length > 100)
                errors.AddError("Score cannot be longer than 100 characters.", nameof(Score));
        });
    }
}