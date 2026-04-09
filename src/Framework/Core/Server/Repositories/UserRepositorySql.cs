using UserSelect = Crudspa.Framework.Core.Server.Sproxies.UserSelect;

namespace Crudspa.Framework.Core.Server.Repositories;

public class UserRepositorySql(ISessionService sessionService) : IUserRepository
{
    public async Task<IList<User>> SelectByIds(String connection, IEnumerable<Guid?> userIds, Guid? portalId = null)
    {
        var users = await UserSelectByIds.Execute(connection, userIds);

        users.Apply(x => x.MaySignIn = true);

        return users;
    }

    public async Task<User?> Select(String connection, Guid? userId, Guid? portalId = null)
    {
        var user = await UserSelect.Execute(connection, userId);

        if (user?.Id is not null)
            user.MaySignIn = true;

        return user;
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user, Guid? portalId)
    {
        if (user.MaySignIn == false)
            return null;

        var userId = await UserInsert.Execute(connection, transaction, sessionId, user, portalId);
        await sessionService.InvalidateAll();
        return userId;
    }

    public async Task<Guid?> Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user, Guid? portalId)
    {
        if (user.MaySignIn == false)
        {
            if (user.Id.HasSomething())
            {
                await UserDelete.Execute(connection, transaction, sessionId, user);
                await sessionService.InvalidateAll();
            }

            return null;
        }

        if (user.Id.HasNothing())
            return await Insert(connection, transaction, sessionId, user, portalId);

        await UserUpdate.Execute(connection, transaction, sessionId, user);
        await sessionService.InvalidateAll();
        return user.Id;
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, User user)
    {
        if (user.Id is null)
            return;

        await UserDelete.Execute(connection, transaction, sessionId, user);
        await sessionService.InvalidateAll();
    }

    public async Task<List<Error>> Validate(String connection, User user, Guid? portalId)
    {
        return await ErrorsEx.Validate(async errors =>
        {
            if (!await UserUsernameIsAvailable.Execute(connection, user.Username, portalId, user.Id))
                errors.AddError("Username is unavailable.");
        });
    }
}