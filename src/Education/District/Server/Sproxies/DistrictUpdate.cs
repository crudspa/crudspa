namespace Crudspa.Education.District.Server.Sproxies;

using District = Shared.Contracts.Data.District;

public static class DistrictUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, District district)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.DistrictUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", district.Id);

        await command.Execute(connection, transaction);
    }
}