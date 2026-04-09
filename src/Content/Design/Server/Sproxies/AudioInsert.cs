namespace Crudspa.Content.Design.Server.Sproxies;

public static class AudioInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioElement audio)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AudioInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", audio.ElementId);
        command.AddParameter("@FileId", audio.FileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}