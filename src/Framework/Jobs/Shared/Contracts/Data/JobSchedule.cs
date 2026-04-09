namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class JobSchedule : Observable, IValidates, INamed
{
    public enum RecurrenceIntervals
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
    }

    public enum RecurrencePatterns { Simple, SpecificTime }

    public enum DayOfWeeks
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }

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

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Config
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DeviceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? RecurrenceAmount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RecurrenceIntervals RecurrenceInterval
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RecurrencePatterns RecurrencePattern
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Day
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DayOfWeeks DayOfWeek
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Hour
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Minute
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Second
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? NextRun
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? LastRun
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? LastStatus
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? LastStatusCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DeviceName
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
            else if (Name!.Length > 50)
                errors.AddError("Name cannot be longer than 50 characters.", nameof(Name));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            if (Config.HasNothing())
                errors.AddError("Config is required.", nameof(Config));

            if (Description.HasNothing())
                errors.AddError("Description is required.", nameof(Description));

            if (!RecurrenceAmount.HasValue)
                errors.AddError("Recurrence Amount is required.", nameof(RecurrenceAmount));

            if (RecurrenceAmount.HasValue && RecurrencePattern is RecurrencePatterns.SpecificTime)
            {
                switch (RecurrenceInterval)
                {
                    case RecurrenceIntervals.Second:
                        if (RecurrenceAmount < 10)
                            errors.AddError("Jobs cannot be scheduled to repeat sooner than every 10 seconds.", nameof(RecurrenceAmount));
                        break;

                    case RecurrenceIntervals.Minute:
                        if (RecurrenceAmount is < 1 or > 60)
                            errors.AddError("Recurrence amount must be between 1 and 60 minutes.", nameof(RecurrenceAmount));
                        break;

                    case RecurrenceIntervals.Hour:
                        if (RecurrenceAmount is < 1 or > 24)
                            errors.AddError("Recurrence amount must be between 1 and 24 hours.", nameof(RecurrenceAmount));
                        break;

                    case RecurrenceIntervals.Day:
                        if (RecurrenceAmount is < 1 or > 31)
                            errors.AddError("Recurrence amount must be between 1 and 31 days.", nameof(RecurrenceAmount));
                        if (Hour is < 0 or > 23)
                            errors.AddError("Hour must be between 0 and 23.", nameof(Hour));
                        if (Minute is < 0 or > 59)
                            errors.AddError("Minute must be between 0 and 59.", nameof(Minute));
                        if (Second is < 0 or > 59)
                            errors.AddError("Second must be between 0 and 59.", nameof(Second));
                        break;

                    case RecurrenceIntervals.Week:
                        if (RecurrenceAmount is < 1 or > 52)
                            errors.AddError("Recurrence amount must be between 1 and 52 weeks.", nameof(RecurrenceAmount));
                        if (Hour is < 0 or > 23)
                            errors.AddError("Hour must be between 0 and 23.", nameof(Hour));
                        if (Minute is < 0 or > 59)
                            errors.AddError("Minute must be between 0 and 59.", nameof(Minute));
                        if (Second is < 0 or > 59)
                            errors.AddError("Second must be between 0 and 59.", nameof(Second));
                        break;

                    case RecurrenceIntervals.Month:
                        if (RecurrenceAmount is < 1 or > 12)
                            errors.AddError("Recurrence amount must be between 1 and 12 months.", nameof(RecurrenceAmount));
                        if (Day is null or < 1 or > 31)
                            errors.AddError("Day must be between 1 and 31.", nameof(Day));
                        if (Hour is < 0 or > 23)
                            errors.AddError("Hour must be between 0 and 23.", nameof(Hour));
                        if (Minute is < 0 or > 59)
                            errors.AddError("Minute must be between 0 and 59.", nameof(Minute));
                        if (Second is < 0 or > 59)
                            errors.AddError("Second must be between 0 and 59.", nameof(Second));
                        break;
                }
            }
        });
    }
}