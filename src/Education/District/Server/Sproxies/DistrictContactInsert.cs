namespace Crudspa.Education.District.Server.Sproxies;

public static class DistrictContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictContact districtContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Title", 50, districtContact.Title);
        command.AddParameter("@ContactId", districtContact.ContactId);
        command.AddParameter("@UserId", districtContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}