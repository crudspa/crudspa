namespace Crudspa.Framework.Core.Server.Sproxies;

public static class IconSelectFull
{
    public static async Task<IList<IconFull>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.IconSelectFull";

        return await command.ReadAll(connection, ReadIcon);
    }

    private static IconFull ReadIcon(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            CssClass = reader.ReadString(2),
        };
    }
}