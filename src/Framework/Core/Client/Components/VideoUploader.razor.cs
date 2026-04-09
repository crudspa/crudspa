using Microsoft.AspNetCore.Components.Forms;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class VideoUploader : IAsyncDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public VideoFile VideoFile { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean Required { get; set; }
    [Parameter] public Int64 MaxFileSize { get; set; } = 4L * 1024L * 1024L * 1024L;

    [Parameter] public String Accept { get; set; } = Constants.Join(Constants.AllowedVideoExtensions, Constants.AllowedVideoContentTypes);
    [Parameter] public G.List<String> AllowedExtensions { get; set; } = [.. Constants.AllowedVideoExtensions];
    [Parameter] public String? UploadPath { get; set; } = "/api/framework/core/video-file/upload";

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] public HttpClient Http { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected override Task OnInitializedAsync()
    {
        VideoFile.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        VideoFile.PropertyChanged -= HandleModelChanged;
        await JsBridge.RevokeObjectUrl(VideoFile.UploadPreview);
    }

    private async Task HandleFileChanged(InputFileChangeEventArgs args)
    {
        var file = args.File;

        if (file.Size > MaxFileSize)
        {
            VideoFile.UploadStatus = $"File is too large ({file.Size:N0} bytes). Max allowed is {MaxFileSize:N0} bytes.";
            return;
        }

        var extension = file.Name.GetExtension();

        if (extension.HasNothing() || !AllowedExtensions.Contains(extension))
        {
            VideoFile.UploadStatus = $"File type '{extension}' not allowed.";
            return;
        }

        try
        {
            await JsBridge.RevokeObjectUrl(VideoFile.UploadPreview);

            VideoFile.Id = null;
            VideoFile.BlobId = null;
            VideoFile.Name = file.Name;
            VideoFile.UploadPreview = null;
            VideoFile.UploadProgress = 10;
            VideoFile.UploadStatus = "Uploading...";

            var inputId = $"input{InstanceId:N}";
            VideoFile.UploadPreview = await JsBridge.GetObjectUrlFromInput(inputId);

            using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var progressStream = new HttpContentEx(uploadStream, file.Size, (bytesSent, totalBytes) =>
            {
                var pct = (Int32)Math.Min(100, Math.Max(0, bytesSent * 100.0 / totalBytes));
                VideoFile.UploadProgress = pct;
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
                VideoFile.UploadStatus = $"Upload failed: {(Int32)response.StatusCode} {response.ReasonPhrase}. {detail}";
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var uploadedFile = json.FromJson<VideoFile>();

                if (uploadedFile is null)
                    VideoFile.UploadStatus = $"Upload failed: server did not send properly formatted response. {json}";
                else
                {
                    VideoFile.UploadStatus = "Upload succeeded.";
                    VideoFile.BlobId = uploadedFile.BlobId;
                    VideoFile.Name = uploadedFile.Name;
                    VideoFile.Format = uploadedFile.Format;
                }
            }
        }
        catch (Exception ex)
        {
            VideoFile.UploadStatus = $"Upload failed: {ex.Message}";
        }
        finally
        {
            VideoFile.UploadProgress = 0;
        }
    }

    private async Task Clear()
    {
        await JsBridge.RevokeObjectUrl(VideoFile.UploadPreview);
        VideoFile.Id = null;
        VideoFile.BlobId = null;
        VideoFile.Name = null;
        VideoFile.UploadPreview = null;
        VideoFile.UploadProgress = 0;
        VideoFile.UploadStatus = null;
    }
}