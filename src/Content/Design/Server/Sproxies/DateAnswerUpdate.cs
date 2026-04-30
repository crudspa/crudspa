namespace Crudspa.Content.Design.Server.Sproxies;

public static class DateAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DateAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.DateAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@DateMin", answer.DateMin);
        command.AddParameter("@DateMax", answer.DateMax);
        command.AddParameter("@TimeMin", answer.TimeMin?.ToTimeSpan());
        command.AddParameter("@TimeMax", answer.TimeMax?.ToTimeSpan());
        command.AddParameter("@DateTimeMin", answer.DateTimeMin);
        command.AddParameter("@DateTimeMax", answer.DateTimeMax);

        await command.Execute(connection, transaction);
    }
}