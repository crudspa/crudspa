namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassRecordingUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassRecording classRecording)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassRecordingUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classRecording.Id);
        command.AddParameter("@AudioFileId", classRecording.AudioFileFile.Id);
        command.AddParameter("@ImageId", classRecording.ImageFile.Id);
        command.AddParameter("@CategoryId", classRecording.CategoryId);
        command.AddParameter("@Unit", classRecording.Unit);
        command.AddParameter("@Lesson", classRecording.Lesson);
        command.AddParameter("@TeacherNotes", classRecording.TeacherNotes);

        await command.Execute(connection, transaction);
    }
}