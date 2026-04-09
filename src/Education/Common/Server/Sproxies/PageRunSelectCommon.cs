using Crudspa.Content.Display.Server;
using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Common.Server.Sproxies;

public static class PageRunSelectCommon
{
    public static async Task<Page?> Execute(String connection, Page page, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.PageRunSelectCommon";

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

            var activityElements = new List<ActivityElement>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                activityElements.Add(ReadActivityElement(reader));

            var activityChoices = new List<ActivityChoice>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                activityChoices.Add(ReadActivityChoice(reader));

            var activityChoicesByActivityId = activityChoices.ToLookup(x => x.ActivityId);

            foreach (var activityElement in activityElements)
                activityElement.Activity!.ActivityChoices = activityChoicesByActivityId[activityElement.ActivityId]
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

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
            var activityElementsByElementId = activityElements.ToLookup(x => x.ElementId);

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

                        case var id when id == ElementTypeIdsCommon.Activity:
                            var activityElement = activityElementsByElementId[element.Id].First();
                            var activitySectionElement = new SectionElement { Element = element };
                            activitySectionElement.SetConfig(activityElement);
                            section.Elements.Add(activitySectionElement);
                            break;
                    }
                }
            }

            return page;
        });
    }

    private static ActivityElement ReadActivityElement(SqlDataReader reader)
    {
        return new()
        {
            Activity = new()
            {
                Id = reader.ReadGuid(0),
                Key = reader.ReadString(1),
                ActivityTypeId = reader.ReadGuid(2),
                ContentAreaId = reader.ReadGuid(3),
                StatusId = reader.ReadGuid(4),
                ContextText = reader.ReadString(5),
                ContextAudioFileId = reader.ReadGuid(6),
                ContextImageFileId = reader.ReadGuid(7),
                ExtraText = reader.ReadString(8),
                ActivityTypeName = reader.ReadString(9),
                ActivityTypeKey = reader.ReadString(10),
                ActivityTypeDisplayView = reader.ReadString(11),
                ActivityTypeCategoryKey = reader.ReadString(12),
                ActivityTypeCategoryName = reader.ReadString(13),
                ActivityTypeShuffleChoices = reader.ReadBoolean(14),
                ContentAreaName = reader.ReadString(15),
                ContentAreaKey = reader.ReadString(16),
                ContentAreaAppNavText = reader.ReadString(17),
                ContextAudioFile = new()
                {
                    Id = reader.ReadGuid(18),
                    BlobId = reader.ReadGuid(19),
                    Name = reader.ReadString(20),
                    Format = reader.ReadString(21),
                    OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(22),
                    OptimizedBlobId = reader.ReadGuid(23),
                    OptimizedFormat = reader.ReadString(24),
                },
                ContextImageFile = new()
                {
                    Id = reader.ReadGuid(25),
                    BlobId = reader.ReadGuid(26),
                    Name = reader.ReadString(27),
                    Format = reader.ReadString(28),
                    Width = reader.ReadInt32(29),
                    Height = reader.ReadInt32(30),
                    Caption = reader.ReadString(31),
                },
                StatusName = reader.ReadString(32),
            },
            Id = reader.ReadGuid(33),
            ElementId = reader.ReadGuid(34),
            ActivityId = reader.ReadGuid(35),
        };
    }

    public static ActivityChoice ReadActivityChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ActivityId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            AudioFileId = reader.ReadGuid(3),
            ImageFileId = reader.ReadGuid(4),
            IsCorrect = reader.ReadBoolean(5),
            Ordinal = reader.ReadInt32(6),
            ColumnOrdinal = reader.ReadInt32(7).GetValueOrDefault(),
            AudioFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(15),
                BlobId = reader.ReadGuid(16),
                Name = reader.ReadString(17),
                Format = reader.ReadString(18),
                Width = reader.ReadInt32(19),
                Height = reader.ReadInt32(20),
                Caption = reader.ReadString(21),
            },
        };
    }
}