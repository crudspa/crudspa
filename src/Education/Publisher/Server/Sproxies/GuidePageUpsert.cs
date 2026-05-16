namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GuidePageUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GuidePage guidePage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GuidePageUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GuideBinderId", guidePage.GuideBinderId);
        command.AddParameter("@PageId", guidePage.PageId);
        command.AddParameter("@ShowGuide", guidePage.ShowGuide ?? false);
        command.AddParameter("@GuideText", guidePage.GuideText);
        command.AddParameter("@GuideAudioId", guidePage.GuideAudioFile.Id);
        command.AddParameter("@ShowNotebook", guidePage.ShowNotebook ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}