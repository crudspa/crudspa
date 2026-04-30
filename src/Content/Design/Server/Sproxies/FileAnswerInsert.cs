namespace Crudspa.Content.Design.Server.Sproxies;

public static class FileAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FileAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FileAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}