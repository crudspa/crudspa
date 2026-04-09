namespace Crudspa.Content.Design.Server.Sproxies;

public static class ButtonElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ButtonElement buttonElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ButtonElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", buttonElement.Id);

        await command.Execute(connection, transaction);
    }
}