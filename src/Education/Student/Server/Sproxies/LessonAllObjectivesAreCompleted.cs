namespace Crudspa.Education.Student.Server.Sproxies;

public static class LessonAllObjectivesAreCompleted
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? lessonId, Guid? objectiveId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.LessonAllObjectivesAreCompleted";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LessonId", lessonId);
        command.AddParameter("@ObjectiveId", objectiveId);

        return await command.ExecuteBoolean(connection, "@AllAreComplete");
    }
}