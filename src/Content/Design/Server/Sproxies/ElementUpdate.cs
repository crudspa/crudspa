namespace Crudspa.Content.Design.Server.Sproxies;

public static class ElementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Element element)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ElementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", element.ElementId);
        command.AddParameter("@TypeId", element.TypeId);
        command.AddParameter("@RequireInteraction", element.RequireInteraction ?? true);
        command.AddParameter("@BoxId", element.Box.Id);
        command.AddParameter("@ItemId", element.Item.Id);
        command.AddParameter("@Ordinal", element.Ordinal);

        await command.Execute(connection, transaction);
    }
}