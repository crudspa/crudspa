namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassRecordingInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassRecording classRecording)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassRecordingInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AudioFileId", classRecording.AudioFileFile.Id);
        command.AddParameter("@ImageId", classRecording.ImageFile.Id);
        command.AddParameter("@CategoryId", classRecording.CategoryId);
        command.AddParameter("@Unit", classRecording.Unit);
        command.AddParameter("@Lesson", classRecording.Lesson);
        command.AddParameter("@TeacherNotes", classRecording.TeacherNotes);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}