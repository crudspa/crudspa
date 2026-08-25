namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class PopulationReconcile
{
    public static async Task<PopulationRefreshResult?> Execute(
        String connection,
        Guid? sessionId,
        Guid membershipId,
        PopulationResult result)
    {
        var members = new System.Data.DataTable();
        members.Columns.Add("Id", typeof(Guid));
        foreach (var member in result.Members.DistinctBy(x => x.ContactId))
            members.Rows.Add(member.ContactId);

        var tokens = new System.Data.DataTable();
        tokens.Columns.Add("Key", typeof(String));
        tokens.Columns.Add("Description", typeof(String));
        tokens.Columns.Add("Ordinal", typeof(Int32));
        foreach (var token in result.Tokens.DistinctBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            tokens.Rows.Add(token.Key, (Object?)token.Description ?? DBNull.Value, token.Ordinal);

        var values = new System.Data.DataTable();
        values.Columns.Add("ContactId", typeof(Guid));
        values.Columns.Add("Key", typeof(String));
        values.Columns.Add("Value", typeof(String));
        foreach (var value in result.TokenValues.DistinctBy(x => (x.ContactId, x.Key)))
            values.Rows.Add(value.ContactId, value.Key, (Object?)value.Value ?? String.Empty);

        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.PopulationReconcile";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", membershipId);
        command.AddStructuredParameter("@Members", "ContentMessaging.GuidList", members);
        command.AddStructuredParameter("@Tokens", "ContentMessaging.PopulationTokenList", tokens);
        command.AddStructuredParameter("@TokenValues", "ContentMessaging.PopulationTokenValueList", values);

        return await command.ReadSingle(connection, reader => new PopulationRefreshResult
        {
            MembershipId = membershipId,
            Added = reader.GetInt32(0),
            Removed = reader.GetInt32(1),
            Preserved = reader.GetInt32(2),
            OptedOut = reader.GetInt32(3),
            Tokens = reader.GetInt32(4),
            TokenValues = reader.GetInt32(5),
        });
    }
}