namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolYearSelectAll
{
    public static async Task<IList<SchoolYear>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolYearSelectAll";

        return await command.ReadAll(connection, ReadSchoolYear);
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