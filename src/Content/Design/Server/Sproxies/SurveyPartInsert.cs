namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyPartInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyPart part)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyPartInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SurveyId", part.SurveyId);
        command.AddParameter("@Title", 75, part.Title);
        command.AddParameter("@Instructions", part.Instructions);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}