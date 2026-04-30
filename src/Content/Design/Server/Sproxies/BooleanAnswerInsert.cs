namespace Crudspa.Content.Design.Server.Sproxies;

public static class BooleanAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BooleanAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BooleanAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Default", answer.Default);
        command.AddParameter("@Orientation", answer.Orientation);
        command.AddParameter("@TrueLabel", 250, answer.TrueLabel);
        command.AddParameter("@FalseLabel", 250, answer.FalseLabel);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}