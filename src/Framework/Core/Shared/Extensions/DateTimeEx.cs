namespace Crudspa.Framework.Core.Shared.Extensions;

public static class DateTimeEx
{
    extension(DateTime dateTime)
    {
        public DateTimeOffset ToDateTimeOffset(TimeSpan offset)
        {
            if (dateTime == DateTime.MinValue)
                return DateTimeOffset.MinValue;

            DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

            return new(dateTime.Ticks, offset);
        }

        public DateOnly ToDateOnly()
        {
            return new(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }

    extension(DateTimeOffset dateTimeOffset)
    {
        public DateOnly ToDateOnly()
        {
            return new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
        }

        public String AsFileName()
        {
            return dateTimeOffset.ToString("yyyy-MM-dd_HHmm-ssfff");
        }
    }

    extension(DateTimeOffset? value)
    {
        public DateTimeOffset? ToLocalTime(String? timeZoneId)
        {
            if (!value.HasValue) return null;
            if (timeZoneId.HasNothing()) return value;

            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(value.Value, timeZoneId);
        }
    }

    extension(TimeSpan timeSpan)
    {
        public TimeOnly ToTimeOnly()
        {
            return new(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }

    extension(DateOnly dateOnly)
    {
        public DateTime ToDateTime()
        {
            return new(dateOnly.Year, dateOnly.Month, dateOnly.Day);
        }
    }

    extension(DateRange? dateRange)
    {
        public void ResolveDates(String timeZoneId)
        {
            if (dateRange is null) return;

            var offset = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId).GetUtcOffset(DateTime.Now);
            var now = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), offset);

            switch (dateRange.Type)
            {
                case DateRange.Types.Any:
                    dateRange.StartDateTimeOffset = null;
                    dateRange.EndDateTimeOffset = null;
                    break;
                case DateRange.Types.Custom:
                    dateRange.StartDateTimeOffset = dateRange.StartDate.ToDateTimeOffset(offset);
                    dateRange.EndDateTimeOffset = dateRange.EndDate.ToDateTimeOffset(offset);
                    break;
                case DateRange.Types.InTheLastDay:
                    dateRange.StartDateTimeOffset = now.AddDays(-1);
                    dateRange.EndDateTimeOffset = now;
                    break;
                case DateRange.Types.InTheLastWeek:
                    dateRange.StartDateTimeOffset = now.AddDays(-7);
                    dateRange.EndDateTimeOffset = now;
                    break;
                case DateRange.Types.InTheLastMonth:
                    dateRange.StartDateTimeOffset = now.AddMonths(-1);
                    dateRange.EndDateTimeOffset = now;
                    break;
                case DateRange.Types.InTheLastThreeMonths:
                    dateRange.StartDateTimeOffset = now.AddMonths(-3);
                    dateRange.EndDateTimeOffset = now;
                    break;
                case DateRange.Types.InTheLastSixMonths:
                    dateRange.StartDateTimeOffset = now.AddMonths(-6);
                    dateRange.EndDateTimeOffset = now;
                    break;
                case DateRange.Types.InTheLastYear:
                    dateRange.StartDateTimeOffset = now.AddYears(-1);
                    dateRange.EndDateTimeOffset = now;
                    break;
                default:
                    throw new($"Date range type not supported. ({dateRange.Type})");
            }
        }
    }
}