using Microsoft.AspNetCore.Components.Forms;

namespace Crudspa.Framework.Core.Client.Components;

public partial class FilePicker
{
    private String InputId { get; } = $"fileInput-{Guid.NewGuid():N}";

    [Parameter] public String? FileName { get; set; }
    [Parameter] public EventCallback<String?> FileNameChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }

    public InputFile FileInputReference { get; set; } = null!;

    private async Task HandleFileSelected(InputFileChangeEventArgs args)
    {
        var file = args.File;
        FileName = file.Name;
        await FileNameChanged.InvokeAsync(FileName);
    }
}