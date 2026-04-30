namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, QuestionElement questionElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", questionElement.ElementId);
        command.AddParameter("@QuestionId", questionElement.QuestionId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}