namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictSelect
{
    public static async Task<District?> Execute(String connection, Guid? sessionId, District district)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", district.Id);

        return await command.ReadSingle(connection, ReadDistrict);
    }

    private static District ReadDistrict(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            StudentIdNumberLabel = reader.ReadString(1),
            AssessmentExplainer = reader.ReadString(2),
            OrganizationId = reader.ReadGuid(3),
            AddressId = reader.ReadGuid(4),
            DistrictContactCount = reader.ReadInt32(5),
            CommunityCount = reader.ReadInt32(6),
            SchoolCount = reader.ReadInt32(7),
        };
    }
}