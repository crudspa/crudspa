namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Stage : Observable, IValidates, INamed, IOrderable
{
    public enum Anchors { CampaignStart, LessonStart, AssessmentStart }
    public enum WeekendAdjustments { NextWeekday, PreviousWeekday, Exact }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CampaignId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Offset
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Anchors Anchor
    {
        get;
        set => SetProperty(ref field, value);
    } = Anchors.LessonStart;

    public WeekendAdjustments WeekendAdjustment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public TimeOnly? SendTime
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? MessageCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!CampaignId.HasValue)
                errors.AddError("Campaign is required.", nameof(CampaignId));

            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));

            if (!Offset.HasValue)
                errors.AddError("Days from start is required.", nameof(Offset));

            if (!SendTime.HasValue)
                errors.AddError("Send time is required.", nameof(SendTime));
        });
    }
}