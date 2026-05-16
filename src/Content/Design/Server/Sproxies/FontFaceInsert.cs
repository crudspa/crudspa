namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontFaceInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFace fontFace)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontFaceInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@FontId", fontFace.FontId);
        command.AddParameter("@FileId", fontFace.FileFile.Id);
        command.AddParameter("@Style", 10, fontFace.Style);
        command.AddParameter("@WeightMin", fontFace.WeightMin);
        command.AddParameter("@WeightMax", fontFace.WeightMax);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}