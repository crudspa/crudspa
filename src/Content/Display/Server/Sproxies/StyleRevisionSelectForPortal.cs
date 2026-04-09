namespace Crudspa.Content.Display.Server.Sproxies;

public static class StyleRevisionSelectForPortal
{
    public static async Task<Int32?> Execute(String connection, Guid portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.StyleRevisionSelectForPortal";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadSingle(connection, reader => reader.ReadInt32(0));
    }
}