namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class DateRange : Observable
{
    public enum Types
    {
        Any,
        Custom,
        InTheLastDay,
        InTheLastWeek,
        InTheLastMonth,
        InTheLastThreeMonths,
        InTheLastSixMonths,
        InTheLastYear,
    }

    public Types Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTime StartDate
    {
        get;
        set => SetProperty(ref field, value);
    } = DateTime.Now.AddDays(-7);

    public DateTime EndDate
    {
        get;
        set => SetProperty(ref field, value);
    } = DateTime.Now;

    public DateTimeOffset? StartDateTimeOffset
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? EndDateTimeOffset
    {
        get;
        set => SetProperty(ref field, value);
    }
}