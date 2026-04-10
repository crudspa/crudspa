# Types | File

File handling is one of the highest-risk parts of CRUD platforms. Uploading bytes is easy. Keeping metadata, optimization, caching, and authorization coherent over time is harder.

Crudspa treats files as first-class typed contracts (`ImageFile`, `AudioFile`, `VideoFile`, `PdfFile`, `FontFile`) and keeps SQL metadata as the source of truth.

## Default Approach

Use this end-to-end flow:

* Upload bytes to framework upload endpoints.
* Persist typed file metadata in framework file tables.
* Store only metadata `Id` foreign keys in domain tables.
* Save/update file metadata in services before parent row sproxies run.
* Render and fetch through typed components and `FetchUrl()` helpers.

## File Catalog

 File Type | SQL Table | Optimization Support | Common Components
 --- | --- | --- | ---
 `ImageFile` | `[Framework].[ImageFile]` | optimized `.webp` + responsive widths | `ImageUploader`, `Image`
 `AudioFile` | `[Framework].[AudioFile]` | optimized `.mp3` | `AudioUploader`, `AudioPlayer`
 `VideoFile` | `[Framework].[VideoFile]` | optimized `.mp4` | `VideoUploader`, `VideoPlayer`
 `PdfFile` | `[Framework].[PdfFile]` | none | `PdfUploader`, `PdfViewer`
 `FontFile` | `[Framework].[FontFile]` | none | `FontUploader`

## Nullability Policy

File references are usually nullable in domain DTOs and tables. This matches common CRUD reality:

* attached media is often optional,
* null file references are legitimate relational states,
* required/optional rules are enforced in validation and service logic, not by over-constraining DTO shapes.

## SQL Modeling

Domain rows hold file metadata foreign keys:

```sql
create table [Education].[Post] (
    [AudioId] uniqueidentifier null,
    [ImageId] uniqueidentifier null,
    [PdfId] uniqueidentifier null,
    [VideoId] uniqueidentifier null,
    constraint [FK_Education_Post_Audio] foreign key ([AudioId]) references [Framework].[AudioFile] ([Id]),
    constraint [FK_Education_Post_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
    constraint [FK_Education_Post_Pdf] foreign key ([PdfId]) references [Framework].[PdfFile] ([Id]),
    constraint [FK_Education_Post_Video] foreign key ([VideoId]) references [Framework].[VideoFile] ([Id])
);
```

Framework tables hold metadata and optimization pointers:

```sql
create table [Framework].[ImageFile] (
    [BlobId] uniqueidentifier not null,
    [Name] nvarchar(150) not null,
    [Format] nvarchar(10) not null,
    [OptimizedStatus] int default(0) not null,
    [OptimizedBlobId] uniqueidentifier null,
    [Resized96BlobId] uniqueidentifier null,
    [Resized192BlobId] uniqueidentifier null,
    [Resized360BlobId] uniqueidentifier null
);
```

## Upload Options

Uploader components expose explicit controls for file behavior:

```csharp
[Parameter] public Int64 MaxFileSize { get; set; } = 4L * 1024L * 1024L * 1024L;
[Parameter] public String Accept { get; set; } = Constants.Join(Constants.AllowedImageExtensions, Constants.AllowedImageContentTypes);
[Parameter] public List<String> AllowedExtensions { get; set; } = [.. Constants.AllowedImageExtensions];
[Parameter] public String? UploadPath { get; set; } = "/api/framework/core/image-file/upload";
```

Framework controllers validate extension and content type before accepting uploads.

## UI Controls

Start with the smallest uploader binding:

```razor
<ImageUploader ReadOnly="Model.ReadOnly"
               Required="false"
               ImageFile="Model.Entity.ImageFile" />
```

Render the same file contract with the matching display component:

```razor
<Image ImageFile="Model.Entity.ImageFile" />
```

The same pattern applies to audio, video, PDF, and font types with their matching uploader/viewer controls.

For the complete media component surface, see [Components | Media](../Components/Media.md).

## Save Integration

In service code, save each file metadata object first, then persist resulting metadata IDs in the parent row:

```csharp
var imageFileResponse = await fileMetadataService.SaveImageFile(new(request.SessionId, post.ImageFile));
if (!imageFileResponse.Ok)
{
    response.AddErrors(imageFileResponse.Errors);
    return null;
}

post.ImageFile.Id = imageFileResponse.Value.Id;
```

Then the parent sproxy writes the file IDs:

```csharp
command.AddParameter("@AudioId", post.AudioFile.Id);
command.AddParameter("@ImageId", post.ImageFile.Id);
command.AddParameter("@PdfId", post.PdfFile.Id);
command.AddParameter("@VideoId", post.VideoFile.Id);
```

`IFileMetadataService` standardizes add/update/remove/clone behavior so file lifecycles stay consistent across modules.

## Fetch And Render

Use media helpers instead of hard-coded URLs:

```csharp
return BuildFetchUrl(
    "/api/framework/core/image-file/fetch",
    imageFileId,
    imageFile.CacheVersion(width),
    ("width", width is > 0 ? $"{width.Value:D}" : null),
    ("download", download ? "true" : null));
```

`FetchUrl()` includes a `version` token so browser caching is strong but still updates when blobs change.

Uploader components expose original download links with `original=true`, while standard rendering usually prefers optimized assets.

File workflows usually cross several steps. The upload updates model-held metadata, the parent save persists the relationship to the owning record, fetch URLs carry cache-version information, and the client can then render optimized or original assets as needed. That's why Crudspa treats file metadata as first-class application data.

## Optimization Behavior

Framework jobs optimize media by file type:

* `OptimizeImages` generates optimized `.webp` plus responsive widths (`96` through `3840`).
* `OptimizeAudio` transcodes to `.mp3` and records optimized metadata.
* `OptimizeVideo` transcodes to `.mp4` and records optimized metadata.

Fetch controllers try optimized candidates first, then fall back to original blobs.

## Security And Tenancy

Framework file controllers are convenience endpoints for metadata ID fetches. They are useful for shared media flows, but domain-specific authorization still belongs in feature services/endpoints.

If media access is sensitive, implement a feature endpoint that:

* resolves tenant/user scope,
* authorizes against the owning entity,
* resolves file IDs only after authorization,
* streams via `IBlobService` after checks pass.

## Cleanup Strategy

Crudspa doesn't force one blob cleanup policy. Teams usually choose one of these, or combine them:

* immediate cleanup during update/remove flows,
* scheduled orphan cleanup jobs,
* retention-based purge policies.

Choose based on compliance, recovery expectations, and storage cost profile.

## Next Steps

* [Types | Relationship](Relationship.md)
* [Components | Media](../Components/Media.md)
* [Concepts | Security](../Concepts/Security.md)
* [Documentation Index](../ReadMe.md)
