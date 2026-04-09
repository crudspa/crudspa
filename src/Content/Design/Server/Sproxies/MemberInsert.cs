namespace Crudspa.Content.Design.Server.Sproxies;

public static class MemberInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Member member)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MemberInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", member.MembershipId);
        command.AddParameter("@Status", (Int32?)member.Status);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}