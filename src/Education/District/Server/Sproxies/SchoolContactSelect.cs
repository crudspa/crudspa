namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactSelect
{
    public static async Task<SchoolContact?> Execute(String connection, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolContact.Id);

        return await command.ReadSingle(connection, ReadSchoolContact);
    }

    private static SchoolContact ReadSchoolContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SchoolId = reader.ReadGuid(1),
            TitleId = reader.ReadGuid(2),
            TitleName = reader.ReadString(3),
            TestAccount = reader.ReadBoolean(4),
            UserId = reader.ReadGuid(5),
            ContactId = reader.ReadGuid(6),
        };
    }
}