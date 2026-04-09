namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserSelectByIds
{
    public static async Task<IList<User>> Execute(String connection, IEnumerable<Guid?> userIds)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserSelectByIds";

        command.AddParameter("@Ids", userIds.Distinct());

        return await command.ExecuteQuery(connection, async reader =>
        {
            var users = new List<User>();

            while (await reader.ReadAsync())
                users.Add(ReadUser(reader));

            await reader.NextResultAsync();

            var userRoles = new List<Selectable>();

            while (await reader.ReadAsync())
                userRoles.Add(reader.ReadSelectable());

            foreach (var user in users)
            foreach (var userRole in userRoles.Where(x => x.RootId.Equals(user.Id)))
                user.Roles.Add(userRole);

            return users;
        });
    }

    private static User ReadUser(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Username = reader.ReadString(1),
            ResetPassword = reader.ReadBoolean(2),
        };
    }
}