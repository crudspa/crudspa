namespace Crudspa.Education.District.Shared.Contracts.Data;

public class AssessmentAssignmentBulk : AssessmentAssignment
{
    public enum Actions { Add, Update, AddAndUpdate }

    public enum Scopes { Classroom, School, Community, EntireDistrict }

    public Actions Action
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Scopes Scope
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CommunityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ClassroomId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? RecordsAdded
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? RecordsUpdated
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public override List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Scope == Scopes.Classroom && !ClassroomId.HasValue)
                errors.AddError("Classroom is required.", nameof(ClassroomId));

            if (Scope == Scopes.Community && !CommunityId.HasValue)
                errors.AddError("Community is required.", nameof(ClassroomId));

            if (Scope == Scopes.School && !SchoolId.HasValue)
                errors.AddError("School is required.", nameof(SchoolId));

            if (!AssessmentId.HasValue)
                errors.AddError("Assessment is required.", nameof(AssessmentId));

            if (!StartAfter.HasValue)
                errors.AddError("Start After is required.", nameof(StartAfter));

            if (!EndBefore.HasValue)
                errors.AddError("End Before is required.", nameof(EndBefore));
        });
    }
}