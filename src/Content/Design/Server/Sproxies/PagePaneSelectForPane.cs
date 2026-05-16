namespace Crudspa.Content.Design.Server.Sproxies;

public static class PagePaneSelectForPane
{
    public static async Task<PagePane?> Execute(String connection, Guid? sessionId, Guid? paneId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PagePaneSelectForPane";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", paneId);

        return await command.ReadSingle(connection, ReadPagePane);
    }

    private static PagePane ReadPagePane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PaneId = reader.ReadGuid(1),
            PageId = reader.ReadGuid(2),
        };
    }
}