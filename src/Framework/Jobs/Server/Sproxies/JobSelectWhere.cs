namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobSelectWhere
{
    public static async Task<IList<Job>> Execute(String connection, JobSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobSelectWhere";

        search.AddedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Types", search.Types);
        command.AddParameter("@AddedStart", search.AddedRange.StartDateTimeOffset);
        command.AddParameter("@AddedEnd", search.AddedRange.EndDateTimeOffset);
        command.AddParameter("@Status", search.Status);
        command.AddParameter("@Devices", search.Devices);
        command.AddParameter("@Schedules", search.Schedules);

        return await command.ReadAll(connection, ReadJob);
    }

    private static Job ReadJob(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            TypeId = reader.ReadGuid(3),
            Description = reader.ReadString(4),
            Added = reader.ReadDateTimeOffset(5),
            Started = reader.ReadDateTimeOffset(6),
            Ended = reader.ReadDateTimeOffset(7),
            StatusId = reader.ReadGuid(8),
            DeviceId = reader.ReadGuid(9),
            ScheduleId = reader.ReadGuid(10),
            TypeName = reader.ReadString(11),
            StatusName = reader.ReadString(12),
            StatusCssClass = reader.ReadString(13),
            DeviceName = reader.ReadString(14),
            ScheduleName = reader.ReadString(15),
        };
    }
}