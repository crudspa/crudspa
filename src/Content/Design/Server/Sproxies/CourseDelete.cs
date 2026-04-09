namespace Crudspa.Content.Design.Server.Sproxies;

public static class CourseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Course course)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CourseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", course.Id);

        await command.Execute(connection, transaction);
    }
}