namespace Crudspa.Content.Design.Server.Sproxies;

public static class ImageDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageElement image)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ImageDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", image.Id);

        await command.Execute(connection, transaction);
    }
}