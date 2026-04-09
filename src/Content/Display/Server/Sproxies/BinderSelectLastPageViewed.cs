namespace Crudspa.Content.Display.Server.Sproxies;

public static class BinderSelectLastPageViewed
{
    public static async Task<Guid?> Execute(String connection, Guid? binderId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BinderSelectLastPageViewed";

        command.AddParameter("@BinderId", binderId);
        command.AddParameter("@SessionId", sessionId);

        var pageIdParam = command.AddOutputParameter("@PageId");

        await command.Execute(connection);

        return pageIdParam.Value is not null && pageIdParam.Value.ToString().HasSomething()
            ? Guid.Parse(pageIdParam.Value.ToString()!)
            : null;
    }
}