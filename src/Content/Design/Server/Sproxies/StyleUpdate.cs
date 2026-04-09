namespace Crudspa.Content.Design.Server.Sproxies;

public static class StyleUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Style style)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.StyleUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", style.Id);
        command.AddParameter("@RuleId", style.RuleId);
        command.AddParameter("@ConfigJson", style.ConfigJson);

        await command.Execute(connection, transaction);
    }
}