namespace Crudspa.Content.Design.Server.Sproxies;

public static class OptionsAnswerChoiceInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, OptionsAnswerChoice choice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.OptionsAnswerChoiceInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@OptionsAnswerId", choice.OptionsAnswerId);
        command.AddParameter("@Text", choice.Text);
        command.AddParameter("@Ordinal", choice.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}