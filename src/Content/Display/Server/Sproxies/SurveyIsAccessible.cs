namespace Crudspa.Content.Display.Server.Sproxies;

public static class SurveyIsAccessible
{
    public static async Task<Boolean> Execute(
        String connection,
        IEnumerable<Guid?> licenseIds,
        Guid? surveyId = null,
        Guid? surveyReplyId = null)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.SurveyIsAccessible";

        command.AddParameter("@LicenseIds", licenseIds);
        command.AddParameter("@SurveyId", surveyId);
        command.AddParameter("@SurveyReplyId", surveyReplyId);

        var output = command.AddOutputParameter("@IsAccessible", System.Data.SqlDbType.Bit);
        await command.Execute(connection);
        return output.Value is true;
    }
}