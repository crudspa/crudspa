using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class OrganizationEdit
{
    [Parameter, EditorRequired] public Organization? Organization { get; set; }
    [Parameter, EditorRequired] public G.List<Named>? PermissionNames { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public BatchModel<Role> RolesModel { get; set; } = new OrganizationRoleBatchModel();

    protected override void OnParametersSet()
    {
        if (Organization is null) return;

        if (!ReferenceEquals(RolesModel.Entities, Organization.Roles))
        {
            RolesModel.Entities = Organization.Roles;

            if (RolesModel.Entities.Count > 0)
                RolesModel.UpdateSort();
        }
    }

    public void AddRole()
    {
        if (Organization is null) return;

        var tempId = Guid.NewGuid();

        var maxOrdinal = Organization.Roles.Count == 0 ? -1 : Organization.Roles.Max(x => x.Ordinal ?? 0);

        var role = new Role
        {
            Id = tempId,
            Name = "New Role",
            Ordinal = maxOrdinal + 1,
        };

        foreach (var permission in PermissionNames!)
            role.Permissions.Add(new()
            {
                Id = permission.Id,
                Name = permission.Name,
                Selected = false,
            });

        Organization.Roles.Add(role);

        ScrollService.ToId(role.Id.GetValueOrDefault());
    }

    public void RemoveRole(Guid? roleId)
    {
        Organization?.Roles.RemoveWhere(x => x.Id.Equals(roleId));
    }
}

public class OrganizationRoleBatchModel() : BatchModel<Role>()
{
    public override String? OrderBy(Role entity)
    {
        return entity.Name;
    }
}