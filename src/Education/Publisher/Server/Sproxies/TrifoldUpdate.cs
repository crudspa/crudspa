namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrifoldUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Trifold trifold)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrifoldUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", trifold.Id);
        command.AddParameter("@StatusId", trifold.StatusId);
        command.AddParameter("@RequiresAchievementId", trifold.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", trifold.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", trifold.Binder.TypeId);

        await command.Execute(connection, transaction);
    }
}