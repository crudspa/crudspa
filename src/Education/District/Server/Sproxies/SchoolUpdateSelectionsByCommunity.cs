namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolUpdateSelectionsByCommunity
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Community community)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolUpdateSelectionsByCommunity";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommunityId", community.Id);
        command.AddParameter("@Schools", community.Schools);

        await command.Execute(connection, transaction);
    }
}