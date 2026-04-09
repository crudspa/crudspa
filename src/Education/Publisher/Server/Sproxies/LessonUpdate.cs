namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LessonUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Lesson lesson)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LessonUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", lesson.Id);
        command.AddParameter("@Title", 75, lesson.Title);
        command.AddParameter("@StatusId", lesson.StatusId);
        command.AddParameter("@ImageId", lesson.ImageFile.Id);
        command.AddParameter("@GuideImageId", lesson.GuideImageFile.Id);
        command.AddParameter("@GuideText", lesson.GuideText);
        command.AddParameter("@GuideAudioId", lesson.GuideAudioFile.Id);
        command.AddParameter("@RequiresAchievementId", lesson.RequiresAchievementId);
        command.AddParameter("@RequireSequentialCompletion", lesson.RequireSequentialCompletion ?? true);
        command.AddParameter("@Treatment", lesson.Treatment ?? true);
        command.AddParameter("@Control", lesson.Control ?? true);

        await command.Execute(connection, transaction);
    }
}