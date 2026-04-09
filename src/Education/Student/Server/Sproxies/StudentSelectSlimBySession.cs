namespace Crudspa.Education.Student.Server.Sproxies;

using Student = Shared.Contracts.Data.Student;

public static class StudentSelectSlimBySession
{
    public static async Task<Student?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentSelectSlimBySession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadStudent);
    }

    private static Student ReadStudent(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            UserId = reader.ReadGuid(2),
            FirstName = reader.ReadString(3),
            LastName = reader.ReadString(4),
            GenderId = reader.ReadGuid(5),
            GradeId = reader.ReadGuid(6),
            AudioGenderId = reader.ReadGuid(7),
            ChallengeLevelId = reader.ReadGuid(8),
            ResearchGroupId = reader.ReadGuid(9),
            PreferredName = reader.ReadString(10),
            AvatarString = reader.ReadString(11),
            TermsAccepted = reader.ReadDateTimeOffset(12),
            IsTestAccount = reader.ReadBoolean(13),
        };
    }
}