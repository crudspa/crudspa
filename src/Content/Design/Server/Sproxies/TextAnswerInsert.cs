namespace Crudspa.Content.Design.Server.Sproxies;

public static class TextAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TextAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TextAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Label", 150, answer.Label);
        command.AddParameter("@Placeholder", 150, answer.Placeholder);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}