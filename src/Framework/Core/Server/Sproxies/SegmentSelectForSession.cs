namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentSelectForSession
{
    private static readonly Comparison<NavSegment> SegmentOrdinalComparison =
        (left, right) => Nullable.Compare(left.Ordinal, right.Ordinal);

    private static readonly Comparison<NavPane> PaneOrdinalComparison =
        (left, right) => Nullable.Compare(left.Ordinal, right.Ordinal);

    public static async Task<IList<NavSegment>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        return await Execute(connection, sessionId, portalId, "FrameworkCore.SegmentSelectForSession");
    }

    public static async Task<IList<NavSegment>> Execute(String connection, Guid? sessionId, Guid? portalId, String commandText)
    {
        await using var command = new SqlCommand();
        command.CommandText = commandText;

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var segments = new List<NavSegment>();

            while (await reader.ReadAsync())
                segments.Add(ReadSegment(reader));

            if (segments.Count == 0)
                return [];

            await reader.NextResultAsync();

            var panesBySegment = new Dictionary<Guid, List<NavPane>>();

            while (await reader.ReadAsync())
            {
                var pane = ReadPane(reader);

                if (pane.SegmentId is null)
                    continue;

                if (!panesBySegment.TryGetValue(pane.SegmentId.Value, out var panes))
                {
                    panes = [];
                    panesBySegment[pane.SegmentId.Value] = panes;
                }

                panes.Add(pane);
            }

            foreach (var panes in panesBySegment.Values)
                panes.Sort(PaneOrdinalComparison);

            var includedSegments = new List<NavSegment>(segments.Count);
            var segmentsById = new Dictionary<Guid, NavSegment>(segments.Count);

            foreach (var segment in segments)
            {
                if (segment.Id is not { } segmentId)
                    continue;

                if (!panesBySegment.TryGetValue(segmentId, out var panes) || panes.Count == 0)
                    continue;

                segment.Panes = panes.ToObservable();
                includedSegments.Add(segment);
                segmentsById[segmentId] = segment;
            }

            var roots = new List<NavSegment>();
            var childrenByParent = new Dictionary<Guid, List<NavSegment>>();

            foreach (var segment in includedSegments)
            {
                if (segment.ParentId is null)
                {
                    roots.Add(segment);
                    continue;
                }

                if (!segmentsById.ContainsKey(segment.ParentId.Value))
                    continue;

                if (!childrenByParent.TryGetValue(segment.ParentId.Value, out var children))
                {
                    children = [];
                    childrenByParent[segment.ParentId.Value] = children;
                }

                children.Add(segment);
            }

            roots.Sort(SegmentOrdinalComparison);

            foreach (var children in childrenByParent.Values)
                children.Sort(SegmentOrdinalComparison);

            AddChildren(roots, childrenByParent);

            return roots;
        });
    }

    private static void AddChildren(IEnumerable<NavSegment>? root, IReadOnlyDictionary<Guid, List<NavSegment>> childrenByParent)
    {
        if (root is null)
            return;

        foreach (var parent in root)
        {
            if (parent.Id is null || !childrenByParent.TryGetValue(parent.Id.Value, out var children))
            {
                parent.Segments = [];
                continue;
            }

            parent.Segments = children.ToObservable();

            AddChildren(children, childrenByParent);
        }
    }

    private static NavSegment ReadSegment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            StatusId = reader.ReadGuid(1),
            PortalId = reader.ReadGuid(2),
            PermissionId = reader.ReadGuid(3),
            ParentId = reader.ReadGuid(4),
            Key = reader.ReadString(5),
            Title = reader.ReadString(6),
            Fixed = reader.ReadBoolean(7),
            RequiresId = reader.ReadBoolean(8),
            TypeId = reader.ReadGuid(9),
            IconId = reader.ReadGuid(10),
            Recursive = reader.ReadBoolean(11),
            Ordinal = reader.ReadInt32(12),
            TypeDisplayView = reader.ReadString(13),
            IconName = reader.ReadString(14),
        };
    }

    private static NavPane ReadPane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            Title = reader.ReadString(2),
            SegmentId = reader.ReadGuid(3),
            TypeId = reader.ReadGuid(4),
            ConfigJson = reader.ReadString(5),
            Ordinal = reader.ReadInt32(6),
            TypeDisplayView = reader.ReadString(7),
        };
    }
}