namespace Crudspa.Content.Design.Server.Sproxies;

public static class ImageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageElement image)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ImageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", image.ElementId);
        command.AddParameter("@FileId", image.FileFile.Id);
        command.AddParameter("@HyperlinkUrl", image.HyperlinkUrl);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}