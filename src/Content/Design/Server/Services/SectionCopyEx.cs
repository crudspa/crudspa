using Crudspa.Content.Display.Shared.Contracts.Config.ElementType;
using Crudspa.Content.Display.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;
using System.Reflection;
using System.Text.Json;

namespace Crudspa.Content.Design.Server.Services;

internal static class SectionCopyEx
{
    public static Section CreateCopy(this Section section, Guid? pageId)
    {
        var copy = section.DeepClone();

        copy.Id = null;
        copy.PageId = pageId;
        copy.Ordinal = null;

        Reset(copy.Box);
        Reset(copy.Container);

        foreach (var element in copy.Elements)
            Reset(element);

        return copy;
    }

    private static void Reset(SectionElement element)
    {
        element.Id = null;
        element.ElementId = null;
        element.SectionId = null;

        Reset(element.Box);
        Reset(element.Item);

        if (element.As<AudioElement>() is { } audio)
        {
            Reset(audio);
            return;
        }

        if (element.As<ButtonElement>() is { } button)
        {
            Reset(button);
            return;
        }

        if (element.As<ImageElement>() is { } image)
        {
            Reset(image);
            return;
        }

        if (element.As<MultimediaElement>() is { } multimedia)
        {
            Reset(multimedia);
            return;
        }

        if (element.As<NoteElement>() is { } note)
        {
            Reset(note);
            return;
        }

        if (element.As<PdfElement>() is { } pdf)
        {
            Reset(pdf);
            return;
        }

        if (element.As<QuestionElement>() is { } question)
        {
            Reset(question);
            return;
        }

        if (element.As<TextElement>() is { } text)
        {
            Reset(text);
            return;
        }

        if (element.As<VideoElement>() is { } video)
        {
            Reset(video);
            return;
        }

        var config = FetchConfig(element);

        if (TryResetConfigIds(config))
        {
            element.ConfigJson = config?.ToJson();
            return;
        }

        throw new($"Element '{element.Id}' is not configured for copy.");
    }

    private static Object? FetchConfig(SectionElement element)
    {
        if (element.Config is not null)
            return element.Config;

        if (element.ConfigType.HasNothing() || element.ConfigJson.HasNothing())
            return null;

        var configType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.GetType(element.ConfigType!))
            .FirstOrDefault(x => x is not null);

        return configType is null
            ? null
            : JsonSerializer.Deserialize(element.ConfigJson, configType);
    }

    private static Boolean TryResetConfigIds(Object? config)
    {
        if (config is null)
            return false;

        var cleared = false;

        foreach (var propertyName in new[] { "Id", "ElementId", "ActivityId" })
        {
            var property = config.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property?.CanWrite != true)
                continue;

            if (property.PropertyType == typeof(Guid?))
            {
                property.SetValue(config, null);
                cleared = true;
                continue;
            }

            if (property.PropertyType == typeof(Guid))
            {
                property.SetValue(config, Guid.Empty);
                cleared = true;
            }
        }

        return cleared;
    }

    private static void Reset(AudioElement audio)
    {
        audio.Id = null;
        audio.ElementId = null;
    }

    private static void Reset(Box box)
    {
        box.Id = null;
    }

    private static void Reset(Button button)
    {
        button.Id = null;
        Reset(button.Box);
    }

    private static void Reset(ButtonElement button)
    {
        button.Id = null;
        button.ElementId = null;
        Reset(button.Button);
    }

    private static void Reset(Container container)
    {
        container.Id = null;
    }

    private static void Reset(ImageElement image)
    {
        image.Id = null;
        image.ElementId = null;
    }

    private static void Reset(Item item)
    {
        item.Id = null;
    }

    private static void Reset(MultimediaElement multimedia)
    {
        multimedia.Id = null;
        multimedia.ElementId = null;
        Reset(multimedia.Container);

        foreach (var multimediaItem in multimedia.MultimediaItems)
        {
            multimediaItem.Id = null;
            multimediaItem.MultimediaElementId = null;

            Reset(multimediaItem.Box);
            Reset(multimediaItem.Item);

            if (multimediaItem.MediaTypeIndex == MultimediaItem.MediaTypes.Button)
                Reset(multimediaItem.Button);
        }
    }

    private static void Reset(NoteElement note)
    {
        note.Id = null;
        note.ElementId = null;

        foreach (var noteImage in note.NoteImages)
        {
            noteImage.Id = null;
            noteImage.NoteId = null;
            noteImage.ImageFileId = null;
        }
    }

    private static void Reset(PdfElement pdf)
    {
        pdf.Id = null;
        pdf.ElementId = null;
    }

    private static void Reset(QuestionElement questionElement)
    {
        questionElement.Id = null;
        questionElement.ElementId = null;
        questionElement.QuestionId = null;

        var question = questionElement.Question;
        question.Id = null;

        question.BooleanAnswer?.ClearIds();
        question.ContactAnswer?.ClearIds();
        question.DateAnswer?.ClearIds();
        question.FileAnswer?.ClearIds();
        question.NumberAnswer?.ClearIds();
        question.OptionsAnswer?.ClearIds();
        question.ScaleAnswer?.ClearIds();
        question.TextAnswer?.ClearIds();
    }

    private static void Reset(TextElement text)
    {
        text.Id = null;
        text.ElementId = null;
    }

    private static void Reset(VideoElement video)
    {
        video.Id = null;
        video.ElementId = null;
    }

    private static void ClearIds(this BooleanAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this ContactAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this DateAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this FileAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this NumberAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this ScaleAnswer answer) { answer.Id = null; answer.QuestionId = null; }
    private static void ClearIds(this TextAnswer answer) { answer.Id = null; answer.QuestionId = null; }

    private static void ClearIds(this OptionsAnswer answer)
    {
        answer.Id = null;
        answer.QuestionId = null;

        foreach (var choice in answer.Choices)
        {
            choice.Id = null;
            choice.OptionsAnswerId = null;
        }
    }
}