namespace Crudspa.Content.Display.Server.Sproxies;

public static class BinderPaneSelectForPane
{
    public static async Task<BinderPane?> Execute(String connection, Guid? sessionId, Guid? paneId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BinderPaneSelectForPane";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", paneId);

        return await command.ReadSingle(connection, ReadBinderPane);
    }

    private static BinderPane ReadBinderPane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PaneId = reader.ReadGuid(1),
            BinderId = reader.ReadGuid(2),
        };
    }
}