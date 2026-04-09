namespace Crudspa.Education.School.Server.Sproxies;

public static class DistrictSelectBySession
{
    public static async Task<District?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.DistrictSelectBySession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadDistrict);
    }

    private static District ReadDistrict(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            StudentIdNumberLabel = reader.ReadString(1),
            AssessmentExplainer = reader.ReadString(2),
            OrganizationName = reader.ReadString(3),
        };
    }
}