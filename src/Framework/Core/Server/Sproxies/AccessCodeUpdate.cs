using System.Data;

namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AccessCodeUpdate
{
    public static async Task<Boolean> Execute(String connection, Guid? userId, AccessCode accessCode)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AccessCodeUpdate";

        command.AddParameter("@UserId", userId);
        command.AddParameter("@Code", accessCode.Code);

        var success = command.AddOutputParameter("@Success", SqlDbType.Bit);

        await command.Execute(connection);

        return Boolean.Parse(success.Value.ToString()!);
    }
}