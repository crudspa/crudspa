namespace Crudspa.Content.Design.Server.Sproxies;

public static class OptionsAnswerChoiceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, OptionsAnswerChoice choice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.OptionsAnswerChoiceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", choice.Id);
        command.AddParameter("@OptionsAnswerId", choice.OptionsAnswerId);
        command.AddParameter("@Text", choice.Text);
        command.AddParameter("@Ordinal", choice.Ordinal);

        await command.Execute(connection, transaction);
    }
}