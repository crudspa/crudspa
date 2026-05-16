namespace Crudspa.Content.Design.Server.Sproxies;

public static class NumberAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NumberAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NumberAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Label", 150, answer.Label);
        command.AddParameter("@IntegerMin", answer.IntegerMin);
        command.AddParameter("@IntegerMax", answer.IntegerMax);
        command.AddParameter("@DecimalMin", answer.DecimalMin);
        command.AddParameter("@DecimalMax", answer.DecimalMax);
        command.AddParameter("@CurrencyMin", answer.CurrencyMin);
        command.AddParameter("@CurrencyMax", answer.CurrencyMax);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}