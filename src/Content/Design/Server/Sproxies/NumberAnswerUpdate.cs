namespace Crudspa.Content.Design.Server.Sproxies;

public static class NumberAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NumberAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NumberAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@IntegerMin", answer.IntegerMin);
        command.AddParameter("@IntegerMax", answer.IntegerMax);
        command.AddParameter("@DecimalMin", answer.DecimalMin);
        command.AddParameter("@DecimalMax", answer.DecimalMax);
        command.AddParameter("@CurrencyMin", answer.CurrencyMin);
        command.AddParameter("@CurrencyMax", answer.CurrencyMax);

        await command.Execute(connection, transaction);
    }
}