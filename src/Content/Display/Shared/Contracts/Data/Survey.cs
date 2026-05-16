using Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Survey : Observable, IValidates
{
    public enum AssignmentKinds { Automatic, Explicit }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PortalKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
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

    public AssignmentKinds AssignmentKind
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PartCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SurveyPart> Parts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public SurveyReply? Reply
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PortalId.HasValue)
                errors.AddError("Portal is required.", nameof(PortalId));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));
        });
    }
}

public class SurveyPart : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SurveyId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SurveyTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Instructions
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? QuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SurveyQuestion> Questions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!SurveyId.HasValue)
                errors.AddError("Survey is required.", nameof(SurveyId));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));
        });
    }
}

public class SurveyQuestion : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PartTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? QuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Question Question
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PartId.HasValue)
                errors.AddError("Part is required.", nameof(PartId));

            errors.AddRange(Question.Validate());
        });
    }
}

public class SurveyReply : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SurveyId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Started
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Completed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Terminated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<QuestionReply> QuestionReplies
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}