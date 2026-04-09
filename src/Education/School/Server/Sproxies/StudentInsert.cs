namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@FirstName", 75, student.FirstName);
        command.AddParameter("@LastName", 75, student.LastName);
        command.AddParameter("@SecretCode", 75, student.SecretCode);
        command.AddParameter("@GradeId", student.GradeId);
        command.AddParameter("@AssessmentLevelGroupId", student.AssessmentLevelGroupId);
        command.AddParameter("@PreferredName", 75, student.PreferredName);
        command.AddParameter("@AvatarString", 2, student.AvatarString);
        command.AddParameter("@IdNumber", 35, student.IdNumber);
        command.AddParameter("@IsTestAccount", student.IsTestAccount);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}