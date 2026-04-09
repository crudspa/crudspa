namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentSelectBySecretCode
{
    public static async Task<Shared.Contracts.Data.Student?> Execute(String connection, String secretCode)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentSelectBySecretCode";

        command.AddParameter("@SecretCode", 75, secretCode);

        return await command.ReadSingle(connection, ReadStudent);
    }

    private static Shared.Contracts.Data.Student ReadStudent(SqlDataReader reader)
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
            PreferredName = reader.ReadString(9),
            AvatarString = reader.ReadString(10),
            TermsAccepted = reader.ReadDateTimeOffset(11),
            IsTestAccount = reader.ReadBoolean(12),
        };
    }
}