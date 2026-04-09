namespace Crudspa.Education.School.Server.Sproxies;

using School = Shared.Contracts.Data.School;

public static class SchoolSelect
{
    public static async Task<School?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadSchool);
    }

    private static School ReadSchool(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}