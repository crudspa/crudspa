namespace Crudspa.Content.Display.Client.Components;

public partial class ForumCommentHeader
{
    [Parameter, EditorRequired] public Comment Comment { get; set; } = null!;
    [Parameter] public String? Title { get; set; }
    [Parameter] public Boolean Pinned { get; set; }

    private String Initials
    {
        get
        {
            var names = (Comment.ByName ?? "Forum participant")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return names.Length switch
            {
                0 => "FP",
                1 => names[0][..1].ToUpperInvariant(),
                _ => $"{names[0][0]}{names[^1][0]}".ToUpperInvariant(),
            };
        }
    }
}