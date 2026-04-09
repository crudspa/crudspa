namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomSelectableStudents
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? sessionId, Guid? classroomId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomSelectableStudents";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ClassroomId", classroomId);

        return await command.ReadSelectables(connection);
    }
}