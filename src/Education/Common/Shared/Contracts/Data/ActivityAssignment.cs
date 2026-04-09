using Crudspa.Education.Common.Shared.Contracts.Ids;

namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ActivityAssignment : Observable, IUnique, IValidates
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

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public DateTimeOffset? Started
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Finished
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityActivityTypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityActivityTypeKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityContentAreaName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityContentAreaKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ActivityChoiceSelection> ActivityChoiceSelections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ActivityTextEntry> TextEntries
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public DateTimeOffset? Assigned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (StatusId.Equals(ActivityAssignmentStatusIds.NotStarted))
            {
                if (Started.HasValue)
                    errors.AddError("The Started field should be empty when the assignment status is 'Not Started'.", nameof(Started));

                if (Finished.HasValue)
                    errors.AddError("The Finished field should be empty when the assignment status is 'Not Started'.", nameof(Finished));

                if (ActivityChoiceSelections.HasItems())
                    errors.AddError("Selections cannot be recorded when the assignment status is 'Not Started'.", nameof(Finished));
            }

            if (StatusId.Equals(ActivityAssignmentStatusIds.Started))
            {
                if (!Started.HasValue)
                    errors.AddError("The Started field must have a value when then assignment status is 'Started'.", nameof(Started));

                if (Finished.HasValue)
                    errors.AddError("The Finished field should be empty when the assignment status is 'Started'.", nameof(Finished));
            }

            if (StatusId.Equals(ActivityAssignmentStatusIds.Successful))
            {
                if (!Started.HasValue)
                    errors.AddError("The Started field must have a value when then assignment status is 'Successful'.", nameof(Started));

                if (!Finished.HasValue)
                    errors.AddError("The Finished field must have a value when then assignment status is 'Successful'.", nameof(Finished));
            }

            if (StatusId.Equals(ActivityAssignmentStatusIds.Failed))
            {
                if (!Started.HasValue)
                    errors.AddError("The Started field must have a value when then assignment status is 'Failed'.", nameof(Started));

                if (!Finished.HasValue)
                    errors.AddError("The Finished field must have a value when then assignment status is 'Failed'.", nameof(Finished));
            }

            ActivityChoiceSelections.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}