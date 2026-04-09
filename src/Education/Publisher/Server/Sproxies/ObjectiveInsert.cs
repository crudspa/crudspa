namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ObjectiveInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Objective objective)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ObjectiveInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LessonId", objective.LessonId);
        command.AddParameter("@Title", 75, objective.Title);
        command.AddParameter("@StatusId", objective.StatusId);
        command.AddParameter("@TrophyImageId", objective.TrophyImageFile.Id);
        command.AddParameter("@RequiresAchievementId", objective.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", objective.GeneratesAchievementId);
        command.AddParameter("@BinderTypeId", objective.Binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}