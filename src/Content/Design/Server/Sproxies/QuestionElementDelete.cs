namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, QuestionElement questionElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", questionElement.Id);

        await command.Execute(connection, transaction);
    }
}