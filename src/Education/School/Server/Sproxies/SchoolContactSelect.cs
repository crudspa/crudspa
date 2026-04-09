namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolContactSelect
{
    public static async Task<SchoolContact?> Execute(String connection, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolContact.Id);

        return await command.ReadSingle(connection, ReadSchoolContact);
    }

    private static SchoolContact ReadSchoolContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TitleId = reader.ReadGuid(1),
            TitleName = reader.ReadString(2),
            TestAccount = reader.ReadBoolean(3),
            ContactId = reader.ReadGuid(4),
            UserId = reader.ReadGuid(5),
        };
    }
}