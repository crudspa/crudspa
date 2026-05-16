namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyPartUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyPart part)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyPartUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", part.Id);
        command.AddParameter("@Title", 75, part.Title);
        command.AddParameter("@Instructions", part.Instructions);

        await command.Execute(connection, transaction);
    }
}