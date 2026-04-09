namespace Crudspa.Education.District.Server.Sproxies;

public static class DistrictContactSelect
{
    public static async Task<DistrictContact?> Execute(String connection, Guid? sessionId, DistrictContact districtContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtContact.Id);

        return await command.ReadSingle(connection, ReadDistrictContact);
    }

    private static DistrictContact ReadDistrictContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            UserId = reader.ReadGuid(2),
            ContactId = reader.ReadGuid(3),
        };
    }
}