namespace Crudspa.Education.District.Server.Sproxies;

public static class DistrictContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictContact districtContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtContact.Id);
        command.AddParameter("@Title", 50, districtContact.Title);
        command.AddParameter("@UserId", districtContact.UserId);

        await command.Execute(connection, transaction);
    }
}