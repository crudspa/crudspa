using System.Text;
using static Crudspa.Framework.Jobs.Shared.Contracts.Data.JobSchedule;

namespace Crudspa.Framework.Jobs.Shared.Extensions;

public static class JobScheduleEx
{
    extension(JobSchedule schedule)
    {
        public DateTimeOffset DetermineNextRunDate()
        {
            var timeZoneInfo = schedule.ResolveTimeZone();
            var now = DateTimeOffset.Now.ConvertTo(timeZoneInfo);

            var recurrenceAmount = schedule.RecurrenceAmount.GetValueOrDefault(1);

            switch (schedule.RecurrencePattern)
            {
                case RecurrencePatterns.Simple:

                    switch (schedule.RecurrenceInterval)
                    {
                        case RecurrenceIntervals.Second:
                            return now.AddSeconds(recurrenceAmount);
                        case RecurrenceIntervals.Minute:
                            return now.AddMinutes(recurrenceAmount);
                        case RecurrenceIntervals.Hour:
                            return now.AddHours(recurrenceAmount);
                        case RecurrenceIntervals.Day:
                            return now.AddDays(recurrenceAmount);
                        case RecurrenceIntervals.Week:
                            return now.AddDays(recurrenceAmount * 7);
                        case RecurrenceIntervals.Month:
                            return now.AddMonths(recurrenceAmount);
                        default:
                            throw new ArgumentOutOfRangeException($"RecurrenceInterval {schedule.RecurrenceInterval} not yet supported");
                    }

                case RecurrencePatterns.SpecificTime:

                    DateTimeOffset nextRun;

                    switch (schedule.RecurrenceInterval)
                    {
                        case RecurrenceIntervals.Day:
                            nextRun = timeZoneInfo.CreateOffsetDateTime(now.Offset, now.Year, now.Month, now.Day,
                                schedule.Hour.GetValueOrDefault(), schedule.Minute.GetValueOrDefault(), schedule.Second.GetValueOrDefault());
                            if (nextRun <= now)
                                nextRun = nextRun.AddDays(recurrenceAmount);
                            break;

                        case RecurrenceIntervals.Week:
                            var dayOfWeek = (Int32)schedule.DayOfWeek;
                            var daysToAdd = (dayOfWeek - (Int32)now.DayOfWeek + 7) % 7;
                            if (daysToAdd == 0 &&
                                new DateTimeOffset(now.Year, now.Month, now.Day,
                                    schedule.Hour.GetValueOrDefault(), schedule.Minute.GetValueOrDefault(), schedule.Second.GetValueOrDefault(), now.Offset) <= now)
                            {
                                daysToAdd = 7;
                            }

                            var nextDate = now.Date.AddDays(daysToAdd + (recurrenceAmount - 1) * 7);
                            nextRun = timeZoneInfo.CreateOffsetDateTime(now.Offset, nextDate.Year, nextDate.Month, nextDate.Day,
                                schedule.Hour.GetValueOrDefault(), schedule.Minute.GetValueOrDefault(), schedule.Second.GetValueOrDefault());
                            break;

                        case RecurrenceIntervals.Month:
                            nextRun = timeZoneInfo.CreateOffsetDateTime(now.Offset, now.Year, now.Month, schedule.Day.GetValueOrDefault(1),
                                schedule.Hour.GetValueOrDefault(), schedule.Minute.GetValueOrDefault(), schedule.Second.GetValueOrDefault());
                            if (nextRun <= now)
                                nextRun = nextRun.AddMonths(recurrenceAmount);
                            break;

                        case RecurrenceIntervals.Hour:
                            nextRun = timeZoneInfo.CreateOffsetDateTime(now.Offset, now.Year, now.Month, now.Day, now.Hour,
                                schedule.Minute.GetValueOrDefault(), schedule.Second.GetValueOrDefault());
                            if (nextRun <= now)
                                nextRun = nextRun.AddHours(recurrenceAmount);
                            break;

                        case RecurrenceIntervals.Minute:
                            nextRun = timeZoneInfo.CreateOffsetDateTime(now.Offset, now.Year, now.Month, now.Day, now.Hour, now.Minute,
                                schedule.Second.GetValueOrDefault());
                            if (nextRun <= now)
                                nextRun = nextRun.AddMinutes(recurrenceAmount);
                            break;

                        case RecurrenceIntervals.Second:
                            nextRun = now.AddSeconds(recurrenceAmount);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException($"RecurrenceInterval {schedule.RecurrenceInterval} not yet supported");
                    }

                    return nextRun;

                default:
                    throw new ArgumentOutOfRangeException($"RecurrencePattern {schedule.RecurrencePattern} not yet supported");
            }
        }

        public String BuildDescription()
        {
            var recurrenceAmount = schedule.RecurrenceAmount.GetValueOrDefault(1);
            var recurrenceInterval = schedule.RecurrenceInterval.ToString();
            var timeZone = schedule.TimeZoneId.HasSomething() ? TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId).StandardName : null;

            var descriptionBuilder = new StringBuilder();

            descriptionBuilder.Append($"Every {recurrenceAmount} ");

            descriptionBuilder.Append(recurrenceAmount > 1 ? $"{recurrenceInterval}s" : recurrenceInterval);

            if (schedule.RecurrencePattern == RecurrencePatterns.SpecificTime)
            {
                descriptionBuilder.Append(' ');

                if (schedule.RecurrenceInterval == RecurrenceIntervals.Week)
                    descriptionBuilder.Append($"on {schedule.DayOfWeek}");
                else if (schedule.RecurrenceInterval == RecurrenceIntervals.Month && schedule.Day.HasValue)
                    descriptionBuilder.Append($"on day {schedule.Day}");

                if (schedule.Hour.HasValue || schedule.Minute.HasValue || schedule.Second.HasValue)
                {
                    descriptionBuilder.Append(" at ");
                    descriptionBuilder.Append(schedule.Hour.GetValueOrDefault(0).ToString("D2"));
                    descriptionBuilder.Append(':');
                    descriptionBuilder.Append(schedule.Minute.GetValueOrDefault(0).ToString("D2"));

                    if (schedule.RecurrenceInterval == RecurrenceIntervals.Second || schedule.Second.HasValue)
                    {
                        descriptionBuilder.Append(':');
                        descriptionBuilder.Append(schedule.Second.GetValueOrDefault(0).ToString("D2"));
                    }
                }

                if (timeZone is not null)
                    descriptionBuilder.Append($" {timeZone}");
            }

            return descriptionBuilder.ToString().Trim();
        }

        public TimeZoneInfo? ResolveTimeZone()
        {
            if (schedule.TimeZoneId.HasNothing())
                return null;

            return TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId);
        }
    }

    extension(DateTimeOffset dateTimeOffset)
    {
        public DateTimeOffset ConvertTo(TimeZoneInfo? timeZoneInfo)
        {
            if (timeZoneInfo is null)
                return dateTimeOffset;

            return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
        }
    }

    extension(TimeZoneInfo? timeZoneInfo)
    {
        public DateTimeOffset CreateOffsetDateTime(TimeSpan offset, Int32 year, Int32 month, Int32 day, Int32 hour, Int32 minute, Int32 second)
        {
            if (timeZoneInfo is null)
                return new(year, month, day, hour, minute, second, offset);

            var localDateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            return new(localDateTime, timeZoneInfo.GetUtcOffset(localDateTime));
        }
    }
}