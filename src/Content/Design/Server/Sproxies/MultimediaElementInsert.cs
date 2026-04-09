namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MultimediaElement multimediaElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", multimediaElement.ElementId);
        command.AddParameter("@ContainerId", multimediaElement.Container?.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}