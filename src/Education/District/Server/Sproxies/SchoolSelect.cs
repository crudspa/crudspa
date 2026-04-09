namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolSelect
{
    public static async Task<School?> Execute(String connection, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);

        return await command.ReadSingle(connection, ReadSchool);
    }

    private static School ReadSchool(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            CommunityId = reader.ReadGuid(2),
            CommunityName = reader.ReadString(3),
            Treatment = reader.ReadBoolean(4),
            AddressId = reader.ReadGuid(5),
            OrganizationId = reader.ReadGuid(6),
            ClassroomCount = reader.ReadInt32(7),
            SchoolContactCount = reader.ReadInt32(8),
        };
    }
}