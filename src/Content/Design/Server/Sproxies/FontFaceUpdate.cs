namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontFaceUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFace fontFace)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontFaceUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", fontFace.Id);
        command.AddParameter("@FileId", fontFace.FileFile.Id);
        command.AddParameter("@Style", 10, fontFace.Style);
        command.AddParameter("@WeightMin", fontFace.WeightMin);
        command.AddParameter("@WeightMax", fontFace.WeightMax);

        await command.Execute(connection, transaction);
    }
}