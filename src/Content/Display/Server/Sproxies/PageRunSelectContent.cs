namespace Crudspa.Content.Display.Server.Sproxies;

public static class PageRunSelectContent
{
    public static async Task<Page?> Execute(String connection, Page page, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PageRunSelectContent";

        command.AddParameter("@Id", page.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            page = PageDataReaders.ReadPage(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                page.Sections.Add(PageDataReaders.ReadSection(reader));

            var elements = new List<Element>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                elements.Add(PageDataReaders.ReadElement(reader));

            var audios = new List<AudioElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                audios.Add(PageDataReaders.ReadAudio(reader));

            var textElements = new List<TextElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                textElements.Add(PageDataReaders.ReadTextElement(reader));

            var images = new List<ImageElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                images.Add(PageDataReaders.ReadImage(reader));

            var buttonElements = new List<ButtonElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                buttonElements.Add(PageDataReaders.ReadButtonElement(reader));

            var multimediaElements = new List<MultimediaElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                multimediaElements.Add(PageDataReaders.ReadMultimediaElement(reader));

            var multimediaItems = new List<MultimediaItem>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                multimediaItems.Add(PageDataReaders.ReadMultimediaItem(reader));

            var notes = new List<NoteElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                notes.Add(PageDataReaders.ReadNote(reader));

            var noteImages = new List<NoteImage>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                noteImages.Add(PageDataReaders.ReadNoteImage(reader));

            var noteImagesByNoteId = noteImages.ToLookup(x => x.NoteId);

            foreach (var note in notes)
                note.NoteImages = noteImagesByNoteId[note.Id].ToObservable();

            var pdfs = new List<PdfElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                pdfs.Add(PageDataReaders.ReadPdf(reader));

            var videos = new List<VideoElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                videos.Add(PageDataReaders.ReadVideo(reader));

            var elementsBySectionId = elements.ToLookup(x => x.SectionId);
            var audiosByElementId = audios.ToLookup(x => x.ElementId);
            var textElementsByElementId = textElements.ToLookup(x => x.ElementId);
            var imagesByElementId = images.ToLookup(x => x.ElementId);
            var buttonElementsByElementId = buttonElements.ToLookup(x => x.ElementId);
            var multimediaElementsByElementId = multimediaElements.ToLookup(x => x.ElementId);
            var multimediaItemsByMultimediaElementId = multimediaItems.ToLookup(x => x.MultimediaElementId);
            var notesByElementId = notes.ToLookup(x => x.ElementId);
            var pdfsByElementId = pdfs.ToLookup(x => x.ElementId);
            var videosByElementId = videos.ToLookup(x => x.ElementId);

            page.Sections = page.Sections.OrderBy(x => x.Ordinal).ToObservable();

            foreach (var section in page.Sections)
            {
                foreach (var element in elementsBySectionId[section.Id].OrderBy(x => x.Ordinal))
                {
                    switch (element.TypeId.GetValueOrDefault())
                    {
                        case var id when id == ElementTypeIds.Audio:
                            var audioElement = audiosByElementId[element.Id].First();
                            var audioSectionElement = new SectionElement { Element = element };
                            audioSectionElement.SetConfig(audioElement);
                            section.Elements.Add(audioSectionElement);
                            break;

                        case var id when id == ElementTypeIds.TextElement:
                            var textElement = textElementsByElementId[element.Id].First();
                            var textSectionElement = new SectionElement { Element = element };
                            textSectionElement.SetConfig(textElement);
                            section.Elements.Add(textSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Image:
                            var imageElement = imagesByElementId[element.Id].First();
                            var imageSectionElement = new SectionElement { Element = element };
                            imageSectionElement.SetConfig(imageElement);
                            section.Elements.Add(imageSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Button:
                            var buttonElement = buttonElementsByElementId[element.Id].First();
                            var buttonSectionElement = new SectionElement { Element = element };
                            buttonSectionElement.SetConfig(buttonElement);
                            section.Elements.Add(buttonSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Multimedia:
                            var multimediaElement = multimediaElementsByElementId[element.Id].First();
                            multimediaElement.MultimediaItems = multimediaItemsByMultimediaElementId[multimediaElement.Id]
                                .OrderBy(x => x.Ordinal)
                                .ToObservable();
                            var multimediaSectionElement = new SectionElement { Element = element };
                            multimediaSectionElement.SetConfig(multimediaElement);
                            section.Elements.Add(multimediaSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Note:
                            var noteElement = notesByElementId[element.Id].First();
                            var noteSectionElement = new SectionElement { Element = element };
                            noteSectionElement.SetConfig(noteElement);
                            section.Elements.Add(noteSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Pdf:
                            var pdfElement = pdfsByElementId[element.Id].First();
                            var pdfSectionElement = new SectionElement { Element = element };
                            pdfSectionElement.SetConfig(pdfElement);
                            section.Elements.Add(pdfSectionElement);
                            break;

                        case var id when id == ElementTypeIds.Video:
                            var videoElement = videosByElementId[element.Id].First();
                            var videoSectionElement = new SectionElement { Element = element };
                            videoSectionElement.SetConfig(videoElement);
                            section.Elements.Add(videoSectionElement);
                            break;
                    }
                }
            }

            return page;
        });
    }
}