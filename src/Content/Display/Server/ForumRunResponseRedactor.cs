using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Server;

public static class ForumRunResponseRedactor
{
    public static void Redact(IEnumerable<Forum>? forums)
    {
        if (forums is null) return;
        foreach (var forum in forums) Redact(forum);
    }

    public static void Redact(Forum? forum)
    {
        if (forum is null) return;
        forum.ImageFile = Safe(forum.ImageFile);
    }

    public static void Redact(IEnumerable<Thread>? threads)
    {
        if (threads is null) return;
        foreach (var thread in threads) Redact(thread);
    }

    public static void Redact(Thread? thread)
    {
        if (thread is null) return;
        Redact(thread.Comment);
    }

    public static void Redact(IEnumerable<Comment>? comments)
    {
        if (comments is null) return;
        foreach (var comment in comments) Redact(comment);
    }

    public static void Redact(Comment? comment)
    {
        if (comment is null) return;

        foreach (var media in comment.CommentMedias)
        {
            media.CommentId = null;
            media.CommentBody = null;
            media.AudioFile = Safe(media.AudioFile);
            media.ImageFile = Safe(media.ImageFile);
            media.PdfFile = Safe(media.PdfFile);
            media.VideoFile = Safe(media.VideoFile);
        }

        Redact(comment.Children);
    }

    private static AudioFile Safe(AudioFile file) => new()
    {
        Name = file.Name,
        Format = file.Format,
    };

    private static ImageFile Safe(ImageFile file) => new()
    {
        Name = file.Name,
        Format = file.Format,
        Width = file.Width,
        Height = file.Height,
        Caption = file.Caption,
    };

    private static PdfFile Safe(PdfFile file) => new()
    {
        Name = file.Name,
        Format = file.Format,
        Description = file.Description,
    };

    private static VideoFile Safe(VideoFile file) => new()
    {
        Name = file.Name,
        Format = file.Format,
        Width = file.Width,
        Height = file.Height,
    };
}