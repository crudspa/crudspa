namespace Crudspa.Framework.Core.Client.Components;

public partial class EmojiDropdown
{
    [Parameter, EditorRequired] public IEnumerable<Emoji> LookupValues { get; set; } = [];
    [Parameter] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String NullText { get; set; } = "[None]";

    protected override void OnParametersSet()
    {
        if (Value is not null && Value.Length > 2)
            throw new ArgumentOutOfRangeException(nameof(Value), Value, "EmojiDropdown expects emojis that fit in nvarchar(2) (<= 2 UTF-16 code units).");

        foreach (var emoji in LookupValues)
        {
            if (emoji?.Character is null)
                throw new ArgumentOutOfRangeException(nameof(LookupValues), "LookupValues contains a null emoji or emoji.Character.");

            if (emoji.Character.Length > 2)
                throw new ArgumentOutOfRangeException(nameof(LookupValues), emoji.Character, "LookupValues contains an emoji that does not fit in nvarchar(2) (<= 2 UTF-16 code units).");
        }
    }

    private Task HandleValueChanged(String? value)
    {
        if (value is not null && value.Length > 2)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Selected emoji does not fit in nvarchar(2) (<= 2 UTF-16 code units).");

        return ValueChanged.InvokeAsync(value);
    }
}