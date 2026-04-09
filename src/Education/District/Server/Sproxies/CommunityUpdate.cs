namespace Crudspa.Education.District.Server.Sproxies;

public static class CommunityUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Community community)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.CommunityUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", community.Id);
        command.AddParameter("@Name", 100, community.Name);

        await command.Execute(connection, transaction);
    }
}