# Components | Media

Media workflows are a common source of duplicated code: upload controls, previews, players, fetch URLs, and modal viewers often diverge across modules.

Crudspa centralizes these patterns with a consistent media component set for image, audio, video, pdf, and font workflows.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `Image`, `ImageDiv` | responsive image rendering (`img` or background image style) | `ImageFile` + media fetch URL extensions
 `ImageUploader`, `AudioUploader`, `VideoUploader`, `PdfUploader`, `FontUploader` | upload workflows with progress and validation | file contracts + upload endpoints + JS drop zone
 `AudioPlayer`, `VideoPlayer` | playback controls and event coordination | `IEventBus`, `IJsBridge`, `SilenceRequested`
 `ImageViewer`, `PdfViewer` | modal viewer bodies | `ImageViewerModel`, `PdfViewerModel`
 `RootModalsCore` | app-level shared host for image/pdf viewer modals | viewer models

## Default Upload Approach

Use media-specific uploaders directly in forms:

```razor
@switch (Model.SelectedMediaType)
{
    case PostEditModel.MediaTypes.Audio:
        <AudioUploader ReadOnly="Model.ReadOnly"
                       Required="false"
                       AudioFile="Model.Entity.AudioFile" />
        break;

    case PostEditModel.MediaTypes.Image:
        <ImageUploader ReadOnly="Model.ReadOnly"
                       Required="false"
                       Width="480"
                       ImageFile="Model.Entity.ImageFile" />
        break;

    case PostEditModel.MediaTypes.Pdf:
        <PdfUploader ReadOnly="Model.ReadOnly"
                     Required="false"
                     PdfFile="Model.Entity.PdfFile" />
        break;

    case PostEditModel.MediaTypes.Video:
        <VideoUploader ReadOnly="Model.ReadOnly"
                       Required="false"
                       VideoFile="Model.Entity.VideoFile" />
        break;
}
```

All uploaders follow the same behavior contract, which keeps module code simple.

## Uploader Option Reference

Shared uploader parameters across media uploaders:

 Parameter | Purpose | Default
 --- | --- | ---
 `ReadOnly` | disables browse/drop and clear actions | `false`
 `Required` | field-level required semantics in surrounding forms | `false`
 `MaxFileSize` | maximum upload size in bytes | `4 GB`
 `Accept` | file input accept string | media-specific defaults
 `AllowedExtensions` | allowed extensions list | media-specific defaults
 `UploadPath` | upload endpoint path | media-specific framework endpoints

Uploader-specific model parameters:

* `ImageUploader` -> `ImageFile`
* `AudioUploader` -> `AudioFile`
* `VideoUploader` -> `VideoFile`
* `PdfUploader` -> `PdfFile`
* `FontUploader` -> `FontFile`

`ImageUploader` also supports `Width` for in-component image preview sizing.

## Upload Lifecycle

All uploaders follow this lifecycle:

1. Validate file size and extension client-side.
2. Reset file model state and set `UploadStatus="Uploading..."`.
3. Create local preview URL with JS bridge.
4. Stream multipart upload with progress updates (`UploadProgress`).
5. Parse server response and copy returned file identity/format fields.
6. Reset progress indicator.

This common flow keeps uploader behavior predictable regardless of media type.

## Player Option Reference

### `AudioPlayer`

 Key Parameters | Purpose
 --- | ---
 `AudioFile` | file to play (required)
 `Control` | `Button` or `Native`
 `PlayText` / `StopText` | button labels
 `PlayIcon` / `StopIcon` | icon classes
 `Compact`, `Hidden`, `Disabled`, `AutoPlay` | presentation/play behavior
 `MediaPlayed` | callback with play metadata

### `VideoPlayer`

 Key Parameters | Purpose
 --- | ---
 `VideoFile` | file to play (required)
 `Poster` | optional poster image
 `Width` | fixed wrapper width
 `CssClass` | additional css class
 `AutoPlay` | start automatically when rendered
 `MediaPlayed` | callback with play metadata

## Viewer Pattern

Use model-backed viewer modals:

```razor
<Modal Model="ImageViewerModel"
       Sizing="Modal.ModalSizing.Fluid"
       Title="View Image">
    <Content>
        <ImageViewer Model="ImageViewerModel" />
    </Content>
</Modal>
```

Or host both image/pdf viewers once at app root with `RootModalsCore`.

## Framework Integration

Media components integrate deeply with framework infrastructure:

* media fetch URLs come from file extension helpers (`FetchUrl` with cache version support)
* uploaders use `IJsBridge` for drag-drop zone and preview URL lifecycle
* uploaders use `HttpClient` + endpoint conventions under `/api/framework/core/...`
* players publish and handle `SilenceRequested` so one media item can silence others
* viewer models inherit `ModalModel`, so waiting/alerts/modal lifecycle are shared

## Practical Guidance

* Keep upload endpoint conventions unless there's a strong reason to customize `UploadPath`.
* Use app-root viewer hosts (`RootModalsCore`) to avoid duplicate modal wiring.
* Use `Image` for responsive media display; use `ImageDiv` only when background-image behavior is required.
* Pass explicit `Width` only when fixed-width media presentation is desired.

## Common Questions

### Can I customize allowed extensions or endpoints?

Yes. Uploaders expose `AllowedExtensions`, `Accept`, and `UploadPath`.

### Should I use raw `<audio>` or `<video>` tags directly?

Use framework players for coordinated playback, event tracking, and consistent behavior.

### Why are uploads model-driven instead of returning raw file ids only?

The file contracts carry display, validation, progress, and optimization metadata that's useful across edit and read flows.

## Tradeoffs

The media stack assumes framework file contracts and API conventions. This is less flexible than hand-rolled upload controls, but dramatically reduces duplicated logic and integration defects.

In practice, the workflow is continuous: upload updates model state and metadata, the parent form saves that metadata, the screen renders previews or players from framework URLs, and shared viewer modals can open when a larger inspection experience is needed. Crudspa treats upload, preview, playback, and viewing as one workflow instead of unrelated controls.

## Next Steps

* [Components | Dialogs](Dialogs.md)
* [Components | Feedback](Feedback.md)
* [Types | File](../Types/File.md)
* [Documentation Index](../ReadMe.md)
