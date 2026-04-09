using System.Collections.ObjectModel;

namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentSelectTree
{
    public static async Task<IList<Expandable>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentSelectTree";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var portals = new List<Portal>();

            while (await reader.ReadAsync())
                portals.Add(ReadPortal(reader));

            await reader.NextResultAsync();

            var segments = new List<Segment>();

            while (await reader.ReadAsync())
                segments.Add(ReadSegment(reader));

            foreach (var portal in portals)
            {
                foreach (var segment in segments.Where(x => x.PortalId.Equals(portal.Id) && x.ParentId is null).OrderBy(x => x.Ordinal))
                    portal.Segments.Add(segment);

                AddChildren(portal.Segments, segments);
            }

            return portals.Select(portal => new Expandable
                {
                    Id = portal.Id,
                    Name = portal.Title,
                    Children = AddChildren(portal.Segments),
                })
                .ToList();
        });
    }

    private static ObservableCollection<Expandable> AddChildren(ObservableCollection<Segment> segments)
    {
        return segments.Select(x => new Expandable
            {
                Id = x.Id,
                Name = x.Title,
                IconCssClass = x.IconCssClass,
                Children = AddChildren(x.Segments),
            })
            .ToObservable();
    }

    private static void AddChildren(IEnumerable<Segment>? root, IEnumerable<Segment>? segments)
    {
        if (root is null || segments is null)
            return;

        foreach (var parent in root)
        {
            parent.Segments = segments
                .Where(x => x.ParentId == parent.Id)
                .OrderBy(x => x.Ordinal)
                .ToObservable();

            AddChildren(parent.Segments, segments);
        }
    }

    private static Portal ReadPortal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
        };
    }

    private static Segment ReadSegment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            Title = reader.ReadString(2),
            PortalId = reader.ReadGuid(3),
            ParentId = reader.ReadGuid(4),
            Ordinal = reader.ReadInt32(5),
            IconCssClass = reader.ReadString(6),
        };
    }
}