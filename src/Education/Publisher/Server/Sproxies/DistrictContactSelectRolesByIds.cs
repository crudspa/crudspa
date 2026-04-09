namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictContactSelectRolesByIds
{
    public static async Task<IList<Role>> Execute(String connection, IEnumerable<Guid?> districtContactIds)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictContactSelectRolesByIds";

        command.AddParameter("@DistrictContactIds", districtContactIds.Distinct());

        return await command.ReadAll(connection, ReadRole);
    }

    private static Role ReadRole(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            ParentId = reader.ReadGuid(2),
        };
    }
}