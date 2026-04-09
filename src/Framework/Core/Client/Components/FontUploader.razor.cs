using Microsoft.AspNetCore.Components.Forms;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class FontUploader : IAsyncDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public FontFile FontFile { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean Required { get; set; }
    [Parameter] public Int64 MaxFileSize { get; set; } = 4L * 1024L * 1024L * 1024L;

    [Parameter] public String Accept { get; set; } = Constants.Join(Constants.AllowedFontExtensions, Constants.AllowedFontContentTypes);
    [Parameter] public G.List<String> AllowedExtensions { get; set; } = [.. Constants.AllowedFontExtensions];
    [Parameter] public String? UploadPath { get; set; } = "/api/framework/core/font-file/upload";

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] public HttpClient Http { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected override Task OnInitializedAsync()
    {
        FontFile.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        FontFile.PropertyChanged -= HandleModelChanged;
        await JsBridge.RevokeObjectUrl(FontFile.UploadPreview);
    }

    private async Task HandleFileChanged(InputFileChangeEventArgs args)
    {
        var file = args.File;

        if (file.Size > MaxFileSize)
        {
            FontFile.UploadStatus = $"File is too large ({file.Size:N0} bytes). Max allowed is {MaxFileSize:N0} bytes.";
            return;
        }

        var extension = file.Name.GetExtension();

        if (extension.HasNothing() || !AllowedExtensions.Contains(extension))
        {
            FontFile.UploadStatus = $"File type '{extension}' not allowed.";
            return;
        }

        try
        {
            await JsBridge.RevokeObjectUrl(FontFile.UploadPreview);

            FontFile.Id = null;
            FontFile.BlobId = null;
            FontFile.Name = file.Name;
            FontFile.UploadPreview = null;
            FontFile.UploadProgress = 10;
            FontFile.UploadStatus = "Uploading...";

            var inputId = $"input{InstanceId:N}";
            FontFile.UploadPreview = await JsBridge.GetObjectUrlFromInput(inputId);

            using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var progressStream = new HttpContentEx(uploadStream, file.Size, (bytesSent, totalBytes) =>
            {
                var pct = (Int32)Math.Min(100, Math.Max(0, bytesSent * 100.0 / totalBytes));
                FontFile.UploadProgress = pct;
            });
            using var content = new MultipartFormDataContent();

            var mediaType = file.ContentType.HasSomething() ? file.ContentType : "application/octet-stream";
            progressStream.Headers.ContentType = new(mediaType);
            content.Add(progressStream, "file", file.Name);

            var uploadUrl = new Uri(new(Navigation.BaseUri), UploadPath!).ToString();

            using var response = await Http.PostAsync(uploadUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var detail = await response.Content.ReadAsStringAsync();
                FontFile.UploadStatus = $"Upload failed: {(Int32)response.StatusCode} {response.ReasonPhrase}. {detail}";
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var uploadedFile = json.FromJson<FontFile>();

                if (uploadedFile is null)
                    FontFile.UploadStatus = $"Upload failed: server did not send properly formatted response. {json}";
                else
                {
                    FontFile.UploadStatus = "Upload succeeded.";
                    FontFile.BlobId = uploadedFile.BlobId;
                    FontFile.Name = uploadedFile.Name;
                    FontFile.Format = uploadedFile.Format;
                }
            }
        }
        catch (Exception ex)
        {
            FontFile.UploadStatus = $"Upload failed: {ex.Message}";
        }
        finally
        {
            FontFile.UploadProgress = 0;
        }
    }

    private async Task Clear()
    {
        await JsBridge.RevokeObjectUrl(FontFile.UploadPreview);
        FontFile.Id = null;
        FontFile.BlobId = null;
        FontFile.Name = null;
        FontFile.UploadPreview = null;
        FontFile.UploadProgress = 0;
        FontFile.UploadStatus = null;
    }
}