namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class EmailTemplate : Observable, IValidates, INamed, ICountable
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MembershipId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Subject
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (Subject.HasNothing())
                errors.AddError("Subject is required.", nameof(Subject));
            else if (Subject!.Length > 150)
                errors.AddError("Subject cannot be longer than 150 characters.", nameof(Subject));

            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));
        });
    }
}