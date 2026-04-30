namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContactAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContactAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Label", 150, answer.Label);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}