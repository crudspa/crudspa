namespace Crudspa.Content.Design.Server.Sproxies;

public static class ImageUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageElement image)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ImageUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", image.Id);
        command.AddParameter("@FileId", image.FileFile.Id);
        command.AddParameter("@HyperlinkUrl", image.HyperlinkUrl);

        await command.Execute(connection, transaction);
    }
}