namespace Crudspa.Education.District.Server.Sproxies;

using District = Shared.Contracts.Data.District;

public static class DistrictSelect
{
    public static async Task<District?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadDistrict);
    }

    private static District ReadDistrict(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}