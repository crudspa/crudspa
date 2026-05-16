namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GuideBinderSelectForBinder
{
    public static async Task<GuideBinder?> Execute(String connection, Guid? sessionId, Guid? binderId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GuideBinderSelectForBinder";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", binderId);

        return await command.ReadSingle(connection, reader => GuideDataReaders.ReadGuideBinder(reader, 0));
    }
}