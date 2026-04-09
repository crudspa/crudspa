namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunityInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Community community)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunityInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictId", community.DistrictId);
        command.AddParameter("@Name", 100, community.Name);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}