namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GuideBinderUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GuideBinder guideBinder)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GuideBinderUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", guideBinder.BinderId);
        command.AddParameter("@GuideImageId", guideBinder.GuideImage.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}