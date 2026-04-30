namespace Crudspa.Content.Design.Server.Sproxies;

public static class DateAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DateAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.DateAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@DateMin", answer.DateMin);
        command.AddParameter("@DateMax", answer.DateMax);
        command.AddParameter("@TimeMin", answer.TimeMin?.ToTimeSpan());
        command.AddParameter("@TimeMax", answer.TimeMax?.ToTimeSpan());
        command.AddParameter("@DateTimeMin", answer.DateTimeMin);
        command.AddParameter("@DateTimeMax", answer.DateTimeMax);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}