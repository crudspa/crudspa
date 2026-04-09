namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContactNotebookSelectForContact
{
    public static async Task<Notebook?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContactNotebookSelectForContact";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var contactNotebook = ReadContactNotebook(reader);

            var notebook = new Notebook
            {
                Id = contactNotebook.NotebookId,
                NotebookId = contactNotebook.NotebookId,
            };

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                notebook.Notepages.Add(ReadNotepage(reader));

            await reader.NextResultAsync();

            var noteImages = new List<NoteImage>();

            while (await reader.ReadAsync())
                noteImages.Add(ReadNoteImage(reader));

            foreach (var notepage in notebook.Notepages)
                notepage.Note!.NoteImages = noteImages.Where(x => x.NoteId.Equals(notepage.NoteId)).ToObservable();

            return notebook;
        });
    }

    private static ContactNotebook ReadContactNotebook(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            NotebookId = reader.ReadGuid(2),
        };
    }

    private static Notepage ReadNotepage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            NotebookId = reader.ReadGuid(1),
            NoteId = reader.ReadGuid(2),
            Text = reader.ReadString(3),
            SelectedImageFileId = reader.ReadGuid(4),
            Ordinal = reader.ReadInt32(5),
            Note = new()
            {
                Id = reader.ReadGuid(6),
                Instructions = reader.ReadString(7),
                ImageFileFile = new()
                {
                    Id = reader.ReadGuid(8),
                    BlobId = reader.ReadGuid(9),
                    Name = reader.ReadString(10),
                    Format = reader.ReadString(11),
                    Width = reader.ReadInt32(12),
                    Height = reader.ReadInt32(13),
                    Caption = reader.ReadString(14),
                },
                RequireText = reader.ReadBoolean(15),
                RequireImageSelection = reader.ReadBoolean(16),
            },
            SelectedImageFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Width = reader.ReadInt32(21),
                Height = reader.ReadInt32(22),
                Caption = reader.ReadString(23),
            },
        };
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