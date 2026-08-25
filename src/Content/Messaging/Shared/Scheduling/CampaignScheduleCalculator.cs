namespace Crudspa.Content.Messaging.Shared.Scheduling;

public static class CampaignScheduleCalculator
{
    public static DateOnly? CalculateDate(
        Stage.Anchors anchor,
        Int32? offset,
        Stage.WeekendAdjustments weekendAdjustment,
        DateOnly? campaignStart,
        DateOnly? lessonStart,
        DateOnly? assessmentStart)
    {
        var start = GetAnchor(anchor, campaignStart, lessonStart, assessmentStart);
        return start.HasValue && offset.HasValue
            ? Adjust(start.Value.AddDays(offset.Value), weekendAdjustment)
            : null;
    }

    public static DateOnly? GetAnchor(
        Stage.Anchors anchor,
        DateOnly? campaignStart,
        DateOnly? lessonStart,
        DateOnly? assessmentStart) => anchor switch
    {
        Stage.Anchors.CampaignStart => campaignStart,
        Stage.Anchors.LessonStart => lessonStart,
        Stage.Anchors.AssessmentStart => assessmentStart,
        _ => null,
    };

    public static String Describe(Stage.Anchors anchor, Int32? offset)
    {
        var start = anchor.GetLabel();
        return offset switch
        {
            null => start,
            0 => $"On {start}",
            < 0 => $"{Math.Abs((Int64)offset.Value)} {DayLabel(offset.Value)} before {start}",
            _ => $"{offset.Value} {DayLabel(offset.Value)} after {start}",
        };
    }

    private static String DayLabel(Int32 days) => Math.Abs((Int64)days) == 1 ? "day" : "days";

    public static DateTimeOffset Calculate(
        DateOnly start,
        Int32 distanceAfterStart,
        TimeOnly sendTime,
        Stage.WeekendAdjustments weekendAdjustment,
        TimeZoneInfo timeZone)
    {
        var date = Adjust(start.AddDays(distanceAfterStart), weekendAdjustment);
        var local = date.ToDateTime(sendTime, DateTimeKind.Unspecified);

        return Convert(local, timeZone);
    }

    public static DateTimeOffset Convert(DateTime local, TimeZoneInfo timeZone)
    {
        local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

        while (timeZone.IsInvalidTime(local))
            local = local.AddMinutes(1);

        var utc = TimeZoneInfo.ConvertTimeToUtc(local, timeZone);
        return new(utc, TimeSpan.Zero);
    }

    public static DateOnly Adjust(DateOnly date, Stage.WeekendAdjustments adjustment)
    {
        if (adjustment == Stage.WeekendAdjustments.Exact)
            return date;

        if (date.DayOfWeek == DayOfWeek.Saturday)
            return adjustment == Stage.WeekendAdjustments.NextWeekday ? date.AddDays(2) : date.AddDays(-1);

        if (date.DayOfWeek == DayOfWeek.Sunday)
            return adjustment == Stage.WeekendAdjustments.NextWeekday ? date.AddDays(1) : date.AddDays(-2);

        return date;
    }
}