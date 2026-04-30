namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Text", question.Text);
        command.AddParameter("@AnswerTypeId", question.AnswerTypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}