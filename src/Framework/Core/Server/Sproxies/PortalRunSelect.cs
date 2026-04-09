namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PortalRunSelect
{
    public static async Task<Portal?> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PortalRunSelect";

        command.AddParameter("@Id", portalId);

        return await command.ReadSingle(connection, ReadPortal);
    }

    private static Portal ReadPortal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            Title = reader.ReadString(2),
            SessionsPersist = reader.ReadBoolean(3),
            AllowSignIn = reader.ReadBoolean(4),
            RequireSignIn = reader.ReadBoolean(5),
            NavigationTypeDisplayView = reader.ReadString(6),
        };
    }
}