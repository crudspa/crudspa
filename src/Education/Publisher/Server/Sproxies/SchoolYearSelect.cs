namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolYearSelect
{
    public static async Task<SchoolYear?> Execute(String connection, SchoolYear schoolYear)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolYearSelect";

        command.AddParameter("@Id", schoolYear.Id);

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