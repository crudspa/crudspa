namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ImageFileInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageFile imageFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ImageFileInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", imageFile.BlobId);
        command.AddParameter("@Name", 150, imageFile.Name?.Trim());
        command.AddParameter("@Format", 10, imageFile.Name.GetExtension());
        command.AddParameter("@Width", imageFile.Width);
        command.AddParameter("@Height", imageFile.Height);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}