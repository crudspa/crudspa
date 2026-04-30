namespace Crudspa.Content.Design.Server.Sproxies;

public static class BooleanAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BooleanAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BooleanAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Default", answer.Default);
        command.AddParameter("@Orientation", answer.Orientation);
        command.AddParameter("@TrueLabel", 250, answer.TrueLabel);
        command.AddParameter("@FalseLabel", 250, answer.FalseLabel);

        await command.Execute(connection, transaction);
    }
}