namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class RolePayload
{
    public Guid? OrganizationId { get; set; }
}

public class RolesChanged : RolePayload;