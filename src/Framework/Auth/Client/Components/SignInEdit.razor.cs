namespace Crudspa.Framework.Auth.Client.Components;

public partial class SignInEdit
{
    [Parameter, EditorRequired] public String Title { get; set; } = null!;
    [Parameter, EditorRequired] public String Audience { get; set; } = null!;
    [Parameter, EditorRequired] public AuthConfig Config { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }

    private AuthPolicy? Policy => Config.Policy(Audience);
    private AuthConnection? Connection => Policy is null ? null : Config.Connection(Policy);
    private String DistrictIdLabel => Connection?.Provider == AuthProviders.Clever
        ? "Clever District ID"
        : "ClassLink Tenant ID";

    private void HandleProviderChanged(String? provider)
    {
        if (Policy is not null)
            Config.SetProvider(Policy, provider);
    }
}