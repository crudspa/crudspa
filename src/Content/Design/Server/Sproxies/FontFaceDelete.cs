namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontFaceDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFace fontFace)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontFaceDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", fontFace.Id);

        await command.Execute(connection, transaction);
    }
}