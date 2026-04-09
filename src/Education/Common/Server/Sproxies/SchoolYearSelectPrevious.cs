namespace Crudspa.Education.Common.Server.Sproxies;

public static class SchoolYearSelectPrevious
{
    public static async Task<SchoolYear?> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.SchoolYearSelectPrevious";

        return await command.ReadSingle(connection, ReadSchoolYear);
    }

    private static SchoolYear ReadSchoolYear(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Starts = reader.ReadDateOnly(2),
            Ends = reader.ReadDateOnly(3),
        };
    }
}