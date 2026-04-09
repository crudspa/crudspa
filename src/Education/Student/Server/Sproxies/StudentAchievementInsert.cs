namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentAchievementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, StudentAchievement studentAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentAchievementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentId", studentAchievement.StudentId);
        command.AddParameter("@AchievementId", studentAchievement.Achievement.Id);
        command.AddParameter("@RelatedEntityId", studentAchievement.RelatedEntityId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}