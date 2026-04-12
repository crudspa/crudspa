namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ImageFileUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageFile imageFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ImageFileUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", imageFile.Id);
        command.AddParameter("@BlobId", imageFile.BlobId);
        command.AddParameter("@Name", 150, imageFile.Name?.Trim());
        command.AddParameter("@Format", 10, imageFile.Name.GetExtension());
        command.AddParameter("@Width", imageFile.Width);
        command.AddParameter("@Height", imageFile.Height);
        command.AddParameter("@Caption", imageFile.Caption);

        await command.Execute(connection, transaction);
    }
}