using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Server;

internal static class ForumRunDataReader
{
    public static Forum ReadForum(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            PortalKey = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            StatusName = reader.ReadString(4),
            Title = reader.ReadString(5),
            Description = reader.ReadString(6),
            ImageFile = new()
            {
                Id = reader.ReadGuid(7),
                BlobId = reader.ReadGuid(8),
                Name = reader.ReadString(9),
                Format = reader.ReadString(10),
                Width = reader.ReadInt32(11),
                Height = reader.ReadInt32(12),
                Caption = reader.ReadString(13),
            },
            Ordinal = reader.ReadInt32(14),
        };
    }

    public static Thread ReadThread(SqlDataReader reader, Boolean includesTotalCount)
    {
        var offset = includesTotalCount ? 1 : 0;

        return new()
        {
            TotalCount = includesTotalCount ? reader.ReadInt32(0) : null,
            Id = reader.ReadGuid(offset),
            ForumId = reader.ReadGuid(offset + 1),
            ForumTitle = reader.ReadString(offset + 2),
            Title = reader.ReadString(offset + 3),
            Pinned = reader.ReadBoolean(offset + 4),
            Comment = new()
            {
                Id = reader.ReadGuid(offset + 5),
                Body = reader.ReadString(offset + 6),
                ById = reader.ReadGuid(offset + 7),
                ByName = reader.ReadString(offset + 8),
                ByOrganizationName = reader.ReadString(offset + 9),
                Posted = reader.ReadDateTimeOffset(offset + 10),
                Edited = reader.ReadDateTimeOffset(offset + 11),
                Removed = reader.ReadBoolean(offset + 12) ?? false,
                CanEdit = reader.ReadBoolean(offset + 15) ?? false,
                CanDelete = reader.ReadBoolean(offset + 16) ?? false,
                CanReply = true,
            },
            CommentCount = reader.ReadInt32(offset + 13),
            LastActivity = reader.ReadDateTimeOffset(offset + 14),
            CanEdit = reader.ReadBoolean(offset + 15) ?? false,
            CanDelete = reader.ReadBoolean(offset + 16) ?? false,
            CanModerate = reader.ReadBoolean(offset + 17) ?? false,
        };
    }

    public static Comment ReadComment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ParentId = reader.ReadGuid(1),
            Body = reader.ReadString(2),
            ById = reader.ReadGuid(3),
            ByName = reader.ReadString(4),
            ByOrganizationName = reader.ReadString(5),
            Posted = reader.ReadDateTimeOffset(6),
            Edited = reader.ReadDateTimeOffset(7),
            ThreadId = reader.ReadGuid(8),
            Removed = reader.ReadBoolean(9) ?? false,
            CanEdit = reader.ReadBoolean(10) ?? false,
            CanDelete = reader.ReadBoolean(11) ?? false,
            CanReply = reader.ReadBoolean(12) ?? false,
        };
    }

    public static CommentMedia ReadCommentMedia(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            CommentId = reader.ReadGuid(1),
            Type = reader.ReadEnum<CommentMedia.Types>(2),
            AudioFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(7),
                OptimizedBlobId = reader.ReadGuid(8),
                OptimizedFormat = reader.ReadString(9),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                Width = reader.ReadInt32(14),
                Height = reader.ReadInt32(15),
                Caption = reader.ReadString(16),
            },
            PdfFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Description = reader.ReadString(21),
            },
            VideoFile = new()
            {
                Id = reader.ReadGuid(22),
                BlobId = reader.ReadGuid(23),
                Name = reader.ReadString(24),
                Format = reader.ReadString(25),
                Width = reader.ReadInt32(26),
                Height = reader.ReadInt32(27),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(28),
                OptimizedBlobId = reader.ReadGuid(29),
                OptimizedFormat = reader.ReadString(30),
            },
            Ordinal = reader.ReadInt32(31),
        };
    }

    public static CommentReaction ReadReaction(SqlDataReader reader)
    {
        return new()
        {
            CommentId = reader.ReadGuid(0),
            Emoji = reader.ReadString(1),
            Count = reader.ReadInt32(2) ?? 0,
            Selected = reader.ReadBoolean(3) ?? false,
        };
    }

    public static ForumBundle ReadForumBundle(SqlDataReader reader, Int32 offset = 0)
    {
        return new()
        {
            BundleId = reader.ReadGuid(offset),
            BundleName = reader.ReadString(offset + 1),
            ThreadRule = reader.ReadEnum<ForumBundle.Rules>(offset + 2),
            CommentRule = reader.ReadEnum<ForumBundle.Rules>(offset + 3),
        };
    }

    public static Selectable ReadTag(SqlDataReader reader, Int32 offset = 0, Boolean selected = true)
    {
        return new()
        {
            Id = reader.ReadGuid(offset),
            Name = reader.ReadString(offset + 1),
            Selected = selected,
        };
    }
}