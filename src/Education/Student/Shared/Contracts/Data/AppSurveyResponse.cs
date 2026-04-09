namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class AppSurveyResponse : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentBatchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? QuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AnswerId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!AssignmentBatchId.HasValue)
                errors.AddError("Assignment Batch is required.", nameof(AssignmentBatchId));

            if (!QuestionId.HasValue)
                errors.AddError("Question is required.", nameof(QuestionId));

            if (!AnswerId.HasValue)
                errors.AddError("Answer is required.", nameof(AnswerId));
        });
    }
}