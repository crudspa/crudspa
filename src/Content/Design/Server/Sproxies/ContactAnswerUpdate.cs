namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContactAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContactAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Label", 150, answer.Label);

        await command.Execute(connection, transaction);
    }
}