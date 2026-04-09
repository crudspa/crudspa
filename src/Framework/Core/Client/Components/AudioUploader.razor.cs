using Microsoft.AspNetCore.Components.Forms;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class AudioUploader : IAsyncDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public AudioFile AudioFile { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean Required { get; set; }
    [Parameter] public Int64 MaxFileSize { get; set; } = 4L * 1024L * 1024L * 1024L;

    [Parameter] public String Accept { get; set; } = Constants.Join(Constants.AllowedAudioExtensions, Constants.AllowedAudioContentTypes);
    [Parameter] public G.List<String> AllowedExtensions { get; set; } = [.. Constants.AllowedAudioExtensions];
    [Parameter] public String? UploadPath { get; set; } = "/api/framework/core/audio-file/upload";

    [Inject] public IJsBridge JsBridge { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] public HttpClient Http { get; set; } = null!;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected override Task OnInitializedAsync()
    {
        AudioFile.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        AudioFile.PropertyChanged -= HandleModelChanged;
        await JsBridge.RevokeObjectUrl(AudioFile.UploadPreview);
    }

    private async Task HandleFileChanged(InputFileChangeEventArgs args)
    {
        var file = args.File;

        if (file.Size > MaxFileSize)
        {
            AudioFile.UploadStatus = $"File is too large ({file.Size:N0} bytes). Max allowed is {MaxFileSize:N0} bytes.";
            return;
        }

        var extension = file.Name.GetExtension();

        if (extension.HasNothing() || !AllowedExtensions.Contains(extension))
        {
            AudioFile.UploadStatus = $"File type '{extension}' not allowed.";
            return;
        }

        try
        {
            await JsBridge.RevokeObjectUrl(AudioFile.UploadPreview);

            AudioFile.Id = null;
            AudioFile.BlobId = null;
            AudioFile.Name = file.Name;
            AudioFile.UploadPreview = null;
            AudioFile.UploadProgress = 10;
            AudioFile.UploadStatus = "Uploading...";

            var inputId = $"input{InstanceId:N}";
            AudioFile.UploadPreview = await JsBridge.GetObjectUrlFromInput(inputId);

            using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var progressStream = new HttpContentEx(uploadStream, file.Size, (bytesSent, totalBytes) =>
            {
                var pct = (Int32)Math.Min(100, Math.Max(0, bytesSent * 100.0 / totalBytes));
                AudioFile.UploadProgress = pct;
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
                AudioFile.UploadStatus = $"Upload failed: {(Int32)response.StatusCode} {response.ReasonPhrase}. {detail}";
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var uploadedFile = json.FromJson<AudioFile>();

                if (uploadedFile is null)
                    AudioFile.UploadStatus = $"Upload failed: server did not send properly formatted response. {json}";
                else
                {
                    AudioFile.UploadStatus = "Upload succeeded.";
                    AudioFile.BlobId = uploadedFile.BlobId;
                    AudioFile.Name = uploadedFile.Name;
                    AudioFile.Format = uploadedFile.Format;
                }
            }
        }
        catch (Exception ex)
        {
            AudioFile.UploadStatus = $"Upload failed: {ex.Message}";
        }
        finally
        {
            AudioFile.UploadProgress = 0;
        }
    }

    private async Task Clear()
    {
        await JsBridge.RevokeObjectUrl(AudioFile.UploadPreview);
        AudioFile.Id = null;
        AudioFile.BlobId = null;
        AudioFile.Name = null;
        AudioFile.UploadPreview = null;
        AudioFile.UploadProgress = 0;
        AudioFile.UploadStatus = null;
    }
}