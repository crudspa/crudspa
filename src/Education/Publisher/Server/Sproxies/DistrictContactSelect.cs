namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictContactSelect
{
    public static async Task<DistrictContact?> Execute(String connection, Guid? sessionId, DistrictContact districtContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtContact.Id);

        return await command.ReadSingle(connection, ReadDistrictContact);
    }

    private static DistrictContact ReadDistrictContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            DistrictId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            UserId = reader.ReadGuid(3),
            ContactId = reader.ReadGuid(4),
            DistrictName = reader.ReadString(5),
        };
    }
}