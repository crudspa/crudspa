namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactSelectRolesByIds
{
    public static async Task<IList<Role>> Execute(String connection, IEnumerable<Guid?> schoolContactIds)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactSelectRolesByIds";

        command.AddParameter("@SchoolContactIds", schoolContactIds.Distinct());

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