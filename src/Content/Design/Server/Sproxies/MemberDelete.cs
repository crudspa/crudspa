namespace Crudspa.Content.Design.Server.Sproxies;

public static class MemberDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Member member)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MemberDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", member.Id);

        await command.Execute(connection, transaction);
    }
}