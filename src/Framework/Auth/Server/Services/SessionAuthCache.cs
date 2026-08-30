using System.Collections.Concurrent;
using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Microsoft.Extensions.Caching.Memory;

namespace Crudspa.Framework.Auth.Server.Services;

public class SessionAuthCache(
    SessionAuthServiceSql sessions,
    IServerConfigService serverConfigService,
    IMemoryCache entries)
{
    private static readonly TimeSpan ValidationLease = TimeSpan.FromMinutes(5);
    private readonly ConcurrentDictionary<Guid, Entry> _policies = new();
    private readonly ConcurrentDictionary<Guid, Lazy<Task<Entry?>>> _refreshes = new();

    private Guid PortalId => serverConfigService.Fetch().PortalId;

    public Task<Boolean> Validate(Guid sessionId, Boolean touch = true) =>
        ValidateCore(sessionId, null, touch);

    public Task<Boolean> Validate(Guid sessionId, Guid authPolicyId, Boolean touch = true) =>
        ValidateCore(sessionId, authPolicyId, touch);

    private async Task<Boolean> ValidateCore(Guid sessionId, Guid? authPolicyId, Boolean touch)
    {
        var now = DateTimeOffset.UtcNow;

        if (entries.TryGetValue<Entry>(sessionId, out var entry) && entry is not null)
        {
            var status = entry.Validate(authPolicyId, now, touch);

            if (status == EntryStatus.Valid)
                return true;

            if (status == EntryStatus.Expired)
            {
                Remove(sessionId);
                await sessions.Revoke(sessionId, PortalId, "expired");
                return false;
            }
        }

        var refresh = _refreshes.GetOrAdd(sessionId, _ => new(() => Refresh(sessionId, authPolicyId, touch)));

        try
        {
            return await refresh.Value is not null;
        }
        finally
        {
            _refreshes.TryRemove(KeyValuePair.Create(sessionId, refresh));
        }
    }

    public void Invalidate(Guid sessionId) => Remove(sessionId);

    public void Invalidate(IList<Guid> authPolicyIds)
    {
        if (authPolicyIds.Count == 0)
            return;

        var ids = authPolicyIds.ToHashSet();

        foreach (var item in _policies)
            if (ids.Contains(item.Value.AuthPolicyId))
                Remove(item.Key);
    }

    private async Task<Entry?> Refresh(Guid sessionId, Guid? authPolicyId, Boolean touch)
    {
        var now = DateTimeOffset.UtcNow;
        var activity = entries.TryGetValue<Entry>(sessionId, out var existing) && existing is not null
            ? existing.LastActivity
            : DateTimeOffset.MinValue;
        var state = await sessions.Validate(sessionId, PortalId, activity);

        if (state is null
            || (authPolicyId is not null && state.AuthPolicyId != authPolicyId)
            || state.LastActivity is null
            || state.IdleExpires is null
            || state.AbsoluteExpires is null)
        {
            Remove(sessionId);
            return null;
        }

        var entry = new Entry(state, now.Add(ValidationLease));

        if (touch)
            entry.Validate(authPolicyId, now, true);

        _policies[sessionId] = entry;
        entries.Set(sessionId, entry, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = entry.AbsoluteExpires,
            Size = 1,
        }.RegisterPostEvictionCallback((key, value, _, _) =>
            _policies.TryRemove(KeyValuePair.Create((Guid)key, (Entry)value!))));
        return entry;
    }

    private void Remove(Guid sessionId)
    {
        entries.Remove(sessionId);
        _policies.TryRemove(sessionId, out _);
    }

    private enum EntryStatus
    {
        Valid,
        Refresh,
        Expired,
    }

    private class Entry(SessionAuthState state, DateTimeOffset refreshAfter)
    {
        private readonly Object _sync = new();
        private DateTimeOffset _lastActivity = state.LastActivity!.Value;
        private DateTimeOffset _idleExpires = state.IdleExpires!.Value;

        public Guid AuthPolicyId { get; } = state.AuthPolicyId!.Value;
        public Int32 IdleTimeoutMinutes { get; } = state.IdleTimeoutMinutes;
        public DateTimeOffset AbsoluteExpires { get; } = state.AbsoluteExpires!.Value;
        public DateTimeOffset RefreshAfter { get; } = refreshAfter;

        public DateTimeOffset LastActivity
        {
            get
            {
                lock (_sync)
                    return _lastActivity;
            }
        }

        public EntryStatus Validate(Guid? authPolicyId, DateTimeOffset now, Boolean touch)
        {
            lock (_sync)
            {
                if ((authPolicyId is not null && AuthPolicyId != authPolicyId) || now >= _idleExpires || now >= AbsoluteExpires)
                    return EntryStatus.Expired;

                if (touch)
                {
                    _lastActivity = now;
                    _idleExpires = DateTimeOffset.Compare(now.AddMinutes(IdleTimeoutMinutes), AbsoluteExpires) < 0
                        ? now.AddMinutes(IdleTimeoutMinutes)
                        : AbsoluteExpires;
                }

                if (now >= RefreshAfter)
                    return EntryStatus.Refresh;

                return EntryStatus.Valid;
            }
        }
    }
}