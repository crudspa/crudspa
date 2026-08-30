using Crudspa.Framework.Core.Shared.BaseClasses;

namespace Crudspa.Education.Rostering.Client.Components;

public partial class RosterEdit
{
    [Parameter, EditorRequired] public RosterConfig Config { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }

    private void Add()
    {
        Config.Sources.Add(new()
        {
            Id = Guid.NewGuid(),
            Provider = RosterProviders.Manual,
            Mode = RosterModes.Disabled,
        });
    }

    private void Remove(RosterSource source) =>
        Config.Sources.Remove(source);

    private static IReadOnlyDictionary<String, String> Modes(RosterSource source) =>
        source.Mode == RosterModes.Authoritative ? RosterModes.Names : RosterModes.EditNames;

    private static void SetProvider(RosterSource source, String? provider)
    {
        source.Provider = provider;

        if (!RosterProviders.UsesTenant(provider))
            source.Tenant = null;

        if (provider != RosterProviders.OneRoster)
            source.ClientId = source.ClientSecret = source.TokenUrl = source.BaseUrl = null;
    }

    private static void SetMode(RosterSource source, String? mode)
    {
        var enableRecurring = source.Mode == RosterModes.Disabled
            && mode != RosterModes.Disabled
            && RosterProviders.IsAutomated(source.Provider);

        source.Mode = mode;

        if (enableRecurring)
            SetRecurring(source, true);
        else if (mode == RosterModes.Disabled)
            SetRecurring(source, false);
    }

    private static void SetRecurring(RosterSource source, Boolean recurring)
    {
        source.Recurring = recurring;

        if (!recurring)
            return;

        var bytes = source.Id!.Value.ToByteArray();
        source.ScheduleHour ??= 1 + bytes[0] % 4;
        source.ScheduleMinute ??= bytes[1] % 60;
        source.ScheduleTimeZoneId ??= Crudspa.Framework.Core.Shared.Constants.DefaultTimeZone;
    }
}