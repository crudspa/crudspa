# Components | Authentication

Authentication UI is one of the fastest ways for CRUD apps to become inconsistent. Sign-in prompts appear in different places, state transitions drift, and sign-in or sign-out behavior gets copied from shell to shell.

Crudspa's built-in auth surface is `SignInEmailTfa`. It combines the auth bar and the modal workflow into one component, backed by `SignInEmailTfaModel`. That keeps auth behavior shell-scoped instead of page-scoped.

The `Catalog` sample intentionally uses a smaller sample-specific `SignInCatalogName` component instead. That's a sample convenience for local learning, not the framework default. The shared auth component documented on this page is still `SignInEmailTfa`, which is what `Composer` and `Consumer` use.

## Component Catalog

| Component | Purpose | Primary Integration |
| --- | --- | --- |
| `SignInEmailTfa` | Renders the sign-in bar, the modal workflow, and account actions for the email-plus-one-time-code flow | `Portal`, `ISessionState`, `IAuthService` |
| `SignInEmailTfaModel` | Owns the auth state machine and modal behavior | `ModalModel`, `ICookieService`, `INavigator`, `ISessionState` |
| `IAuthService` | Provides the remote auth boundary for credentials, codes, password reset, password change, and sign-out | hub proxy plus server auth service |

## Default Approach

`SignInEmailTfa` is meant to live at the shell level. The host supplies the current `Portal`, and the component handles both the modal flow and the signed-in or signed-out bar state.

The public component shape is small:

```csharp
[Parameter, EditorRequired] public Portal Portal { get; set; } = null!;
[Parameter] public RenderFragment? LeftPanel { get; set; }
[Parameter] public RenderFragment? MenuItems { get; set; }

protected override async Task OnInitializedAsync()
{
    Model = new(ScrollService, AuthService, CookieService, Navigator, SessionState, Portal.AllowSignIn);

    if (Portal.RequireSignIn == true)
        await Model.ShowIfNeeded();
}
```

Inside the component, the auth modal and the bar stay together on purpose. The modal handles the workflow states, and the bar exposes shell actions such as account settings, change password, and sign-out.

## State Flow

`SignInEmailTfaModel` inherits `ModalModel`, so waiting state, alerts, and show or hide behavior are already part of the flow.

| State | Trigger | Typical Next State |
| --- | --- | --- |
| `EnterCredentials` | `ShowEnterCredentials()` | `EnterCode` on successful credential check |
| `EnterCode` | `ShowEnterCode()` or successful credentials check | hidden modal, or `ChangePassword` when the session requires reset |
| `ResetPassword` | `ShowResetPassword()` | `EnterCode` after the reset request succeeds |
| `ChangePassword` | `ShowChangePassword()` | hidden modal after completion |
| `SignOut` | `ShowSignOut()` | `navigator.Bounce()` after sign-out |

## Parameters

| Parameter | Purpose | Use When |
| --- | --- | --- |
| `Portal` | drives `AllowSignIn`, `RequireSignIn`, and other portal-aware auth behavior | always required |
| `LeftPanel` | extra shell content before the auth controls | tenant switcher, environment badge, or shell-specific status |
| `MenuItems` | extra signed-in menu items between the built-in defaults | app-specific account actions |

## Framework Integration

`SignInEmailTfaModel` wires directly into the core framework pieces that shape auth:

* `IAuthService` checks credentials, verifies access codes, resets passwords, changes passwords, and signs out.
* `ISessionState` initializes signed-in session data after code acceptance.
* `ICookieService` persists the remembered username and the session cookie behavior.
* `INavigator` routes account settings to `/settings?pane=account` and bounces after sign-out.
* `ModalModel` and `ScreenModel` provide `WithWaiting`, alerts, and modal visibility behavior.

That's why auth should stay centralized. The shell needs one place that understands both the modal workflow and the session refresh that follows it.

## Provider Policies

`Framework.Auth` adds a provider-neutral policy layer above the familiar native flows. A portal policy selects one enabled connection for an audience, such as a staff, student, or application-specific portal key. An audience is a lowercase portal key, so production portals use their runtime keys and the samples use `catalog`, `composer`, and `consumer`. The policy holds the session limits and fallback behavior; the connection holds the provider and tenant or district identifier. Authentication and roster provisioning remain independent concerns that meet only at the local user record.

The built-in native choices are Password + Email Code, Email Code, and Student Code. External connections are extensible. The repository includes the `Crudspa.Integrations.Clever` adapter as a concrete OAuth/OIDC example, while ClassLink is represented by the same provider contract and policy vocabulary. A connection is enabled only when its host has valid server-side configuration; client IDs, client secrets, access tokens, and connection strings do not belong in source, seed data, or browser code.

For external sign-in, an application host delegates to a shared Auth host. The Auth host validates a short-lived transaction, normalizes the provider identity, verifies its preprovisioned local-user link and the selected policy, and creates a short-lived, one-time handoff for the destination portal. The destination host redeems that handoff and creates its own server-authenticated session. This keeps provider callbacks, credentials, and cross-host redirects out of the application UI.

## Host Setup

An interactive portal opts into secure authentication with `AddPortalAuth` and `UsePortalAuth`. The host registers `SessionAuthHubFilter` with SignalR and exposes the framework's portal-auth controller. The shared Auth host uses `AddAuth` and registers the chosen external adapters. The public `Samples/Auth` host shows that boundary on local port 42400; its Clever adapter is disabled unless an operator supplies the required private configuration outside source control.

The existing `SignInEmailTfa` component remains the native compatibility UI. Its result contract can redirect to an enabled external policy, while the same component continues the native code flow when policy selects a native method.

## Practical Guidance

* Keep one `SignInEmailTfa` instance per shell, not per page.
* Use `MenuItems` to extend the signed-in menu instead of forking the whole component.
* Let `Portal.RequireSignIn` decide whether the component should prompt on startup.
* Keep the built-in state flow unless you are intentionally replacing the whole auth experience.

## Tradeoffs

The built-in auth flow is opinionated. You get consistent behavior and less maintenance, but fewer local UX variations. That's usually a good trade for admin-style and portal-style applications where session, navigation, and account actions need to stay coherent.

## Next Steps

* [Components | Dialogs](Dialogs.md)
* [Components | Feedback](Feedback.md)
* [Concepts | Security](../Concepts/Security.md)
* [Concepts | Sessions](../Concepts/Sessions.md)
* [Concepts | Rostering](../Concepts/Rostering.md)
* [Documentation Index](../ReadMe.md)
