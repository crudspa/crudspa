using Crudspa.Education.School.Shared.Extensions;

namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSelect
{
    public static async Task<Student?> Execute(String connection, Guid? sessionId, Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", student.Id);

        return await command.ReadSingle(connection, ReadStudent);
    }

    private static Student ReadStudent(SqlDataReader reader)
    {
        var student = new Student
        {
            Id = reader.ReadGuid(0),
            FirstName = reader.ReadString(1),
            LastName = reader.ReadString(2),
            SecretCode = reader.ReadString(3),
            GradeId = reader.ReadGuid(4),
            AssessmentLevelGroupId = reader.ReadGuid(5),
            PreferredName = reader.ReadString(6),
            AvatarString = reader.ReadString(7),
            IdNumber = reader.ReadString(8),
            IsTestAccount = reader.ReadBoolean(9),
            FamilySchoolId = reader.ReadGuid(10),
        };

        student.FriendlyAssessmentLevel = student.AssessmentLevelGroupId.ToFriendlyName();

        return student;
    }
}