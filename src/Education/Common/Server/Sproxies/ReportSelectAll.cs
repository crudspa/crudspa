namespace Crudspa.Education.Common.Server.Sproxies;

public static class ReportSelectAll
{
    public static async Task<IList<Report>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ReportSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadReport);
    }

    private static Report ReadReport(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            IconId = reader.ReadGuid(1),
            DisplayView = reader.ReadString(2),
            Name = reader.ReadString(3),
            Description = reader.ReadString(4),
            Ordinal = reader.ReadInt32(5),
            IconCssClass = reader.ReadString(6),
        };
    }
}