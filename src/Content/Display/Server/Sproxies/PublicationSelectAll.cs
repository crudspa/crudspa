namespace Crudspa.Content.Display.Server.Sproxies;

public static class PublicationSelectAll
{
    public static async Task<IList<PublicationSummary>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PublicationSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var publications = new List<PublicationSummary>();
            while (await reader.ReadAsync()) publications.Add(ReadPublication(reader));
            await reader.NextResultAsync();
            var resources = new List<PublicationResourceSummary>();
            while (await reader.ReadAsync()) resources.Add(ReadPublicationResource(reader));
            var byPublication = resources.ToLookup(x => x.PublicationId);
            foreach (var publication in publications)
                publication.PublicationResources = byPublication[publication.Id].ToObservable();
            return publications;
        });
    }

    private static PublicationSummary ReadPublication(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            GroupId = reader.ReadGuid(2),
            GroupName = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            Citation = reader.ReadString(6),
            Venue = reader.ReadString(7),
            Sample = reader.ReadString(8),
            Timeline = reader.ReadString(9),
            Outcome = reader.ReadString(10),
            Intervention = reader.ReadString(11),
            ImageFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            ImageAlt = reader.ReadString(19),
            Year = reader.ReadInt32(20),
            Ordinal = reader.ReadInt32(21),
            GroupOrdinal = reader.ReadInt32(22),
        };
    }
    private static PublicationResourceSummary ReadPublicationResource(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PublicationId = reader.ReadGuid(1),
            PublicationTitle = reader.ReadString(2),
            TypeId = reader.ReadGuid(3),
            TypeName = reader.ReadString(4),
            Label = reader.ReadString(5),
            Url = reader.ReadString(6),
            PdfFile = new()
            {
                Id = reader.ReadGuid(7),
                BlobId = reader.ReadGuid(8),
                Name = reader.ReadString(9),
                Format = reader.ReadString(10),
                Description = reader.ReadString(11),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            Ordinal = reader.ReadInt32(19),
        };
    }
}