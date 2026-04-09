namespace Crudspa.Content.Design.Server.Sproxies;

public static class ButtonElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ButtonElement buttonElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ButtonElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", buttonElement.ElementId);
        command.AddParameter("@ButtonId", buttonElement.Button.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}