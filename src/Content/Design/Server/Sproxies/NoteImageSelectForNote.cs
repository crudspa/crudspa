namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteImageSelectForNote
{
    public static async Task<IList<NoteImage>> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? noteId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteImageSelectForNote";

        command.AddParameter("@NoteId", noteId);

        return await command.ExecuteAndReadAll(connection, transaction, ReadNoteImage);
    }

    public static async Task<IList<NoteImage>> Execute(String connection, Guid? noteId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteImageSelectForNote";

        command.AddParameter("@NoteId", noteId);

        return await command.ReadAll(connection, ReadNoteImage);
    }

    private static NoteImage ReadNoteImage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            NoteId = reader.ReadGuid(1),
            ImageFileId = reader.ReadGuid(2),
            ImageFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Width = reader.ReadInt32(7),
                Height = reader.ReadInt32(8),
                Caption = reader.ReadString(9),
            },
            Ordinal = reader.ReadInt32(10),
        };
    }
}