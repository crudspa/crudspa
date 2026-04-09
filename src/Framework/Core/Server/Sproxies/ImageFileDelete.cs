namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ImageFileDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageFile imageFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ImageFileDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", imageFile.Id);

        await command.Execute(connection, transaction);
    }
}