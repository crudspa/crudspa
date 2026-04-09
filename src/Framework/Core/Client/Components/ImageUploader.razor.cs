using Microsoft.AspNetCore.Components.Forms;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class ImageUploader : IAsyncDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ImageFile ImageFile { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean Required { get; set; }
    [Parameter] public Int32? Width { get; set; }
    [Parameter] public Int64 MaxFileSize { get; set; } = 4L * 1024L * 1024L * 1024L;

    [Parameter] public String Accept { get; set; } = Constants.Join(Constants.AllowedImageExtensions, Constants.AllowedImageContentTypes);
    [Parameter] public G.List<String> AllowedExtensions { get; set; } = [.. Constants.AllowedImageExtensions];
    [Parameter] public String? UploadPath { get; set; } = "/api/framework/core/image-file/upload";

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] public HttpClient Http { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected override Task OnInitializedAsync()
    {
        ImageFile.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        ImageFile.PropertyChanged -= HandleModelChanged;
        await JsBridge.RevokeObjectUrl(ImageFile.UploadPreview);
    }

    private async Task HandleFileChanged(InputFileChangeEventArgs args)
    {
        var file = args.File;

        if (file.Size > MaxFileSize)
        {
            ImageFile.UploadStatus = $"File is too large ({file.Size:N0} bytes). Max allowed is {MaxFileSize:N0} bytes.";
            return;
        }

        var extension = file.Name.GetExtension();

        if (extension.HasNothing() || !AllowedExtensions.Contains(extension))
        {
            ImageFile.UploadStatus = $"File type '{extension}' not allowed.";
            return;
        }

        try
        {
            await JsBridge.RevokeObjectUrl(ImageFile.UploadPreview);

            ImageFile.Id = null;
            ImageFile.BlobId = null;
            ImageFile.Name = file.Name;
            ImageFile.Caption = null;
            ImageFile.UploadPreview = null;
            ImageFile.UploadProgress = 10;
            ImageFile.UploadStatus = "Uploading...";

            var inputId = $"input{InstanceId:N}";
            ImageFile.UploadPreview = await JsBridge.GetObjectUrlFromInput(inputId);

            using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var progressStream = new HttpContentEx(uploadStream, file.Size, (bytesSent, totalBytes) =>
            {
                var pct = (Int32)Math.Min(100, Math.Max(0, bytesSent * 100.0 / totalBytes));
                ImageFile.UploadProgress = pct;
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
                ImageFile.UploadStatus = $"Upload failed: {(Int32)response.StatusCode} {response.ReasonPhrase}. {detail}";
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var uploadedFile = json.FromJson<ImageFile>();

                if (uploadedFile is null)
                    ImageFile.UploadStatus = $"Upload failed: server did not send properly formatted response. {json}";
                else
                {
                    ImageFile.UploadStatus = "Upload succeeded.";
                    ImageFile.BlobId = uploadedFile.BlobId;
                    ImageFile.Name = uploadedFile.Name;
                    ImageFile.Format = uploadedFile.Format;
                }
            }
        }
        catch (Exception ex)
        {
            ImageFile.UploadStatus = $"Upload failed: {ex.Message}";
        }
        finally
        {
            ImageFile.UploadProgress = 0;
        }
    }

    private async Task Clear()
    {
        await JsBridge.RevokeObjectUrl(ImageFile.UploadPreview);
        ImageFile.Id = null;
        ImageFile.BlobId = null;
        ImageFile.Name = null;
        ImageFile.Caption = null;
        ImageFile.UploadPreview = null;
        ImageFile.UploadProgress = 0;
        ImageFile.UploadStatus = null;
    }
}