namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSelectableClassrooms
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? sessionId, Guid? studentId, Guid? schoolYearId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSelectableClassrooms";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentId", studentId);
        command.AddParameter("@SchoolYearId", schoolYearId);

        return await command.ReadSelectables(connection);
    }
}