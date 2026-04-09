namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageSelectForBinder
{
    public static async Task<IList<Page>> Execute(String connection, Guid? sessionId, Guid? binderId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageSelectForBinder";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", binderId);

        return await command.ReadAll(connection, PageSelect.ReadPage);
    }
}