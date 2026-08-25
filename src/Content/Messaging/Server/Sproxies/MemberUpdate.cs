namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MemberUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Member member)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MemberUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", member.Id);
        command.AddParameter("@Status", (Int32?)member.Status);

        await command.Execute(connection, transaction);
    }
}