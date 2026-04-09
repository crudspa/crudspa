namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolSelect
{
    public static async Task<School?> Execute(String connection, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);

        return await command.ReadSingle(connection, ReadSchool);
    }

    private static School ReadSchool(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            DistrictId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            CommunityId = reader.ReadGuid(3),
            CommunityName = reader.ReadString(4),
            Treatment = reader.ReadBoolean(5),
            OrganizationId = reader.ReadGuid(6),
            AddressId = reader.ReadGuid(7),
            SchoolContactCount = reader.ReadInt32(8),
        };
    }
}