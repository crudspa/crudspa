namespace Crudspa.Content.Design.Server.Sproxies;

public static class ElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Element element)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SectionId", element.SectionId);
        command.AddParameter("@TypeId", element.TypeId);
        command.AddParameter("@RequireInteraction", element.RequireInteraction ?? true);
        command.AddParameter("@BoxId", element.Box.Id);
        command.AddParameter("@ItemId", element.Item.Id);
        command.AddParameter("@Ordinal", element.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}