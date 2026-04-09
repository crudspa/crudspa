namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ObjectiveUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Objective objective)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ObjectiveUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", objective.Id);
        command.AddParameter("@Title", 75, objective.Title);
        command.AddParameter("@StatusId", objective.StatusId);
        command.AddParameter("@TrophyImageId", objective.TrophyImageFile.Id);
        command.AddParameter("@RequiresAchievementId", objective.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", objective.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", objective.Binder.TypeId);

        await command.Execute(connection, transaction);
    }
}