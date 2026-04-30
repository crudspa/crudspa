namespace Crudspa.Content.Design.Server.Sproxies;

public static class OptionsAnswerChoiceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, OptionsAnswerChoice choice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.OptionsAnswerChoiceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", choice.Id);

        await command.Execute(connection, transaction);
    }
}