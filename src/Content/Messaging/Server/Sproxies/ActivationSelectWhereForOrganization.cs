namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class ActivationSelectWhereForOrganization
{
    public static async Task<IList<Activation>> Execute(String connection, Guid? sessionId, ActivationSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.ActivationSelectWhereForOrganization";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@OrganizationId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadActivation);
    }

    private static Activation ReadActivation(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            OrganizationId = reader.ReadGuid(3),
            CampaignId = reader.ReadGuid(4),
            CampaignName = reader.ReadString(5),
            BatchId = reader.ReadGuid(6),
            Start = reader.ReadDateOnly(7),
            Activated = reader.ReadDateTimeOffset(8),
            ActivatedBy = reader.ReadGuid(9),
        };
    }
}