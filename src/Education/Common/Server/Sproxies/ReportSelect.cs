namespace Crudspa.Education.Common.Server.Sproxies;

public static class ReportSelect
{
    public static async Task<Report?> Execute(String connection, Guid? sessionId, Report portal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ReportSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", portal.Id);

        return await command.ReadSingle(connection, ReadReport);
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