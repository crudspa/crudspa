using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Framework.Core.Client.Components;

public static class ContentStatusColors
{
    public static String? For(Guid? id, String? name = null)
    {
        if (id.Equals(ContentStatusIds.Draft))
            return "content-draft";

        if (id.Equals(ContentStatusIds.Complete))
            return "content-complete";

        if (id.Equals(ContentStatusIds.Retired))
            return "content-retired";

        return name?.Trim().ToLowerInvariant() switch
        {
            "draft" => "content-draft",
            "complete" => "content-complete",
            "retired" => "content-retired",
            _ => null,
        };
    }
}