namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentAchievementExists
{
    public static async Task<Boolean> Execute(String connection, StudentAchievement studentAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentAchievementExists";

        command.AddParameter("@StudentId", studentAchievement.StudentId);
        command.AddParameter("@AchievementId", studentAchievement.Achievement.Id);

        return await command.ExecuteBoolean(connection, "@AlreadyUnlocked");
    }
}