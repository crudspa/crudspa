namespace Crudspa.Education.School.Server.Sproxies;

public static class AssessmentSelect
{
    public static async Task<Assessment?> Execute(String connection, Assessment assessment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.AssessmentSelect";

        command.AddParameter("@Id", assessment.Id);

        return await command.ReadSingle(connection, ReadAssessment);
    }

    private static Assessment ReadAssessment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            AvailableStart = reader.ReadDateOnly(3),
            AvailableEnd = reader.ReadDateOnly(4),
        };
    }
}