namespace Crudspa.Education.District.Server.Sproxies;

public static class CommunityDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Community community)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.CommunityDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", community.Id);

        await command.Execute(connection, transaction);
    }
}