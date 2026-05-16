namespace Crudspa.Content.Design.Server.Sproxies;

public static class CoursePaneUpdate
{
    public static async Task Execute(String connection, Guid? sessionId, CoursePane coursePane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CoursePaneUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", coursePane.PaneId);
        command.AddParameter("@IdSource", coursePane.IdSource);
        command.AddParameter("@CourseId", coursePane.CourseId);

        await command.Execute(connection);
    }
}