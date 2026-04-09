namespace Crudspa.Framework.Core.Client.Models;

public class RoleModel : Observable, IDisposable, INamed
{
    private Role _role;

    public String? Name => Role.Name;

    public RoleModel(Role role)
    {
        _role = role;
        _role.PropertyChanged += HandleRoleChanged;
    }

    public void Dispose()
    {
        _role.PropertyChanged -= HandleRoleChanged;
    }

    private void HandleRoleChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Role));
    }

    public Guid? Id
    {
        get => _role.Id;
        set => _role.Id = value;
    }

    public Role Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }
}