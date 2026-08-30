using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.BaseClasses;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;
using System.Collections.ObjectModel;

namespace Crudspa.Framework.Auth.Shared.Contracts.Data;

public class AuthConfig : Observable, IValidates
{
    public ObservableCollection<AuthConnection> Connections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<AuthPolicy> Policies
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public AuthConnection? Connection(AuthPolicy policy) =>
        Connections.FirstOrDefault(x => x.Id == policy.AuthConnectionId);

    public AuthPolicy? Policy(String audience) =>
        Policies.FirstOrDefault(x => x.Audience == audience);

    public void SetProvider(AuthPolicy policy, String? provider)
    {
        var previousId = policy.AuthConnectionId;
        var connection = Connections.FirstOrDefault(x => x.Provider == provider);

        if (connection is null && provider.HasSomething())
        {
            connection = new()
            {
                Id = Guid.NewGuid(),
                Provider = provider,
            };
            Connections.Add(connection);
        }

        policy.AuthConnectionId = connection?.Id;

        if (previousId is not null
            && Policies.All(x => x.AuthConnectionId != previousId)
            && Connections.FirstOrDefault(x => x.Id == previousId) is { } previous)
            Connections.Remove(previous);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            foreach (var connection in Connections)
                errors.AddRange(connection.Validate());

            foreach (var policy in Policies)
                errors.AddRange(policy.Validate());

            if (Policies.GroupBy(x => x.Audience).Any(x => x.Count() > 1))
                errors.AddError("Only one sign-in policy can be configured for each portal.");

            if (Policies.Any(x => Connections.All(connection => connection.Id != x.AuthConnectionId)))
                errors.AddError("Every sign-in policy must use a configured connection.");

            if (Policies.Any(policy => policy.Audience != AuthAudiences.Student
                && Connection(policy)?.Provider == AuthProviders.StudentCode))
                errors.AddError("Student Code can only be used by the Student portal.");

            if (Policies.Any(policy => policy.Audience == AuthAudiences.Student
                && Connection(policy) is { Provider: var provider }
                && provider != AuthProviders.StudentCode
                && !AuthProviders.IsExternal(provider)))
                errors.AddError("Student sign-in must use Student Code or a district provider.");
        });
    }

    public List<Error> Validate(params String[] requiredAudiences)
    {
        var errors = Validate();

        foreach (var audience in requiredAudiences.Where(audience => Policies.Count(x => x.Audience == audience) != 1))
            errors.AddError($"Exactly one {audience} sign-in policy is required.");

        if (Policies.Any(x => !requiredAudiences.Contains(x.Audience)))
            errors.AddError("This record contains a sign-in policy for another portal.");

        return errors;
    }

    public static AuthConfig ForDistrict() => New(
        (AuthAudiences.District, AuthProviders.PasswordEmailCode, 4320, 10080, false),
        (AuthAudiences.School, AuthProviders.PasswordEmailCode, 10080, 20160, true),
        (AuthAudiences.Student, AuthProviders.StudentCode, 720, 1440, false));

    public static AuthConfig ForPublisher() => New(
        (AuthAudiences.Publisher, AuthProviders.PasswordEmailCode, 4320, 10080, false));

    private static AuthConfig New(params (String Audience, String Provider, Int32 Idle, Int32 Absolute, Boolean Persist)[] defaults)
    {
        var config = new AuthConfig();

        foreach (var item in defaults)
        {
            var connection = config.Connections.FirstOrDefault(x => x.Provider == item.Provider);

            if (connection is null)
            {
                connection = new()
                {
                    Id = Guid.NewGuid(),
                    Provider = item.Provider,
                };
                config.Connections.Add(connection);
            }

            config.Policies.Add(new()
            {
                Id = Guid.NewGuid(),
                AuthConnectionId = connection.Id,
                Audience = item.Audience,
                IdleTimeoutMinutes = item.Idle,
                AbsoluteTimeoutMinutes = item.Absolute,
                Persist = item.Persist,
            });
        }

        return config;
    }
}