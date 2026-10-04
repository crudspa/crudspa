namespace Crudspa.Content.Design.Server.Sproxies;

using Publication = Shared.Contracts.Data.Publication;

public static class PublicationSelect
{
    public static async Task<Publication?> Execute(String connection, Guid? sessionId, Publication publication)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publication.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            publication = ReadPublication(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                publication.PublicationResources.Add(ReadPublicationResource(reader));

            return publication;
        });
    }

    private static Publication ReadPublication(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            GroupId = reader.ReadGuid(3),
            GroupName = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            StatusName = reader.ReadString(6),
            Citation = reader.ReadString(7),
            Venue = reader.ReadString(8),
            Sample = reader.ReadString(9),
            Timeline = reader.ReadString(10),
            Outcome = reader.ReadString(11),
            Intervention = reader.ReadString(12),
            ImageFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                Width = reader.ReadInt32(17),
                Height = reader.ReadInt32(18),
                Caption = reader.ReadString(19),
            },
            ImageAlt = reader.ReadString(20),
            Year = reader.ReadInt32(21),
            Ordinal = reader.ReadInt32(22),
        };
    }

    private static PublicationResource ReadPublicationResource(SqlDataReader reader)
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