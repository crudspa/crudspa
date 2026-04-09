namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneTypeSelectFull
{
    public static async Task<IList<PaneTypeFull>> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneTypeSelectFull";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadPaneType);
    }

    private static PaneTypeFull ReadPaneType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            EditorView = reader.ReadString(2),
            DisplayView = reader.ReadString(3),
        };
    }
}