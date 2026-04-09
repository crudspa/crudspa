namespace Crudspa.Framework.Core.Server.Repositories;

public static class UsaPostalRepositorySql
{
    public static async Task<IList<UsaPostal>> SelectByIds(String connection, IEnumerable<Guid?> usaPostalIds, Guid? portalId)
    {
        var usaPostals = await UsaPostalSelectByIds.Execute(connection, usaPostalIds);
        return usaPostals;
    }

    public static async Task<UsaPostal?> Select(String connection, Guid? usaPostalId, Guid? portalId)
    {
        var usaPostal = await UsaPostalSelect.Execute(connection, usaPostalId);
        return usaPostal;
    }

    public static async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal, Guid? portalId = null)
    {
        if (!HasContent(usaPostal))
            return null;

        usaPostal.Id = await UsaPostalInsert.Execute(connection, transaction, sessionId, usaPostal);
        return usaPostal.Id;
    }

    public static async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal, Guid? portalId)
    {
        if (!HasContent(usaPostal))
            return;

        if (usaPostal.Id is null)
        {
            usaPostal.Id = await UsaPostalInsert.Execute(connection, transaction, sessionId, usaPostal);
            return;
        }

        await UsaPostalUpdate.Execute(connection, transaction, sessionId, usaPostal);
    }

    public static async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal)
    {
        if (usaPostal.Id is null)
            return;

        await UsaPostalDelete.Execute(connection, transaction, sessionId, usaPostal);
    }

    public static async Task<List<Error>> Validate(String connection, UsaPostal usaPostal, Guid? portalId = null)
    {
        return await ErrorsEx.Validate(async errors =>
        {
            await Task.CompletedTask;
        });
    }

    private static Boolean HasContent(UsaPostal usaPostal) =>
        usaPostal.RecipientName.HasSomething()
        || usaPostal.BusinessName.HasSomething()
        || usaPostal.StreetAddress.HasSomething()
        || usaPostal.City.HasSomething()
        || usaPostal.PostalCode.HasSomething()
        || usaPostal.StateId is not null;
}