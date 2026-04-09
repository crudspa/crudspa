namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolContactSelect
{
    public static async Task<SchoolContact?> Execute(String connection, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolContactSelect";

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
            Treatment = reader.ReadBoolean(5),
            ContactId = reader.ReadGuid(6),
            UserId = reader.ReadGuid(7),
            SchoolName = reader.ReadString(8),
            DistrictId = reader.ReadGuid(9),
            DistrictName = reader.ReadString(10),
        };
    }
}