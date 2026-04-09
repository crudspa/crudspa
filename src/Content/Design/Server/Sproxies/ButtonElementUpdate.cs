namespace Crudspa.Content.Design.Server.Sproxies;

public static class ButtonElementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ButtonElement buttonElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ButtonElementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", buttonElement.Id);
        command.AddParameter("@ButtonId", buttonElement.Button.Id);

        await command.Execute(connection, transaction);
    }
}