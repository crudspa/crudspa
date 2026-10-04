namespace Crudspa.Content.Design.Server.Services;

using Publication = Shared.Contracts.Data.Publication;

public class PublicationServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IPublicationService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Publication>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Publication>>(request, async response =>
        {
            var publications = await PublicationSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return publications;
        });
    }

    public async Task<Response<Publication?>> Fetch(Request<Publication> request)
    {
        return await wrappers.Try<Publication?>(request, async response =>
        {
            var publication = await PublicationSelect.Execute(Connection, request.SessionId, request.Value);

            return publication;
        });
    }

    public async Task<Response<Publication?>> Add(Request<Publication> request)
    {
        return await wrappers.Validate<Publication?, Publication>(request, async response =>
        {
            var publication = request.Value;

            if (!await PublicationSelectAccess.Execute(Connection, request.SessionId, publication.PortalId, publication.GroupId))
                throw new InvalidOperationException("The publication site or group is outside your scope.");
            if (!await SaveFiles(request.SessionId, publication, null, response)) return null;
            publication.PublicationResources.EnsureOrder();

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await PublicationInsert.Execute(connection, transaction, request.SessionId, publication);

                foreach (var publicationResource in publication.PublicationResources)
                {
                    publicationResource.PublicationId = id;
                    await PublicationResourceInsertByBatch.Execute(connection, transaction, request.SessionId, publicationResource);
                }

                return new Publication
                {
                    Id = id,
                    PortalId = publication.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Publication> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var publication = request.Value;

            var existing = await PublicationSelect.Execute(Connection, request.SessionId, publication);

            if (existing is null || publication.PortalId != existing.PortalId
                || !await PublicationSelectAccess.Execute(Connection, request.SessionId, existing.PortalId, publication.GroupId))
                throw new InvalidOperationException("The publication site or group is outside your scope.");
            foreach (var resource in publication.PublicationResources) resource.PublicationId = existing.Id;
            if (!await SaveFiles(request.SessionId, publication, existing, response)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await PublicationUpdate.Execute(connection, transaction, request.SessionId, publication);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.PublicationResources,
                    publication.PublicationResources,
                    PublicationResourceInsertByBatch.Execute,
                    PublicationResourceUpdateByBatch.Execute,
                    PublicationResourceDeleteByBatch.Execute);

                publication.PublicationResources.EnsureOrder();
                await PublicationResourceUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, publication.PublicationResources);
            });
        });
    }

    public async Task<Response> Remove(Request<Publication> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publication = request.Value;
            var existing = await PublicationSelect.Execute(Connection, request.SessionId, publication);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var publicationResource in existing.PublicationResources)
                    await PublicationResourceDeleteByBatch.Execute(connection, transaction, request.SessionId, publicationResource);

                await PublicationDelete.Execute(connection, transaction, request.SessionId, publication);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Publication>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publications = request.Value;

            publications.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PublicationUpdateOrdinals.Execute(connection, transaction, request.SessionId, publications);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchPublicationGroupNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            ((await PublicationGroupSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id)).Select(x => new Orderable { Id = x.Id, Name = x.Name, Ordinal = x.Ordinal }).ToList()));
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Framework.Core.Server.Sproxies.ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Content.Design.Server.Sproxies.PublicationResourceTypeSelectOrderables.Execute(Connection, request.SessionId));
    }
    private async Task<Boolean> SaveFiles(Guid? sessionId, Publication publication, Publication? existing, Response response)
    {
        // Preserve old files: replacements get new identities and do not delete blobs referenced by history or other entries.
        if (publication.ImageFile.BlobId.HasValue && (publication.ImageFile.Id != existing?.ImageFile.Id || publication.ImageFile.BlobId != existing?.ImageFile.BlobId))
        {
            var saved = await fileService.SaveImage(new(sessionId, publication.ImageFile));
            if (!saved.Ok) { response.AddErrors(saved.Errors); return false; }
            publication.ImageFile = saved.Value!;
        }
        foreach (var resource in publication.PublicationResources)
        {
            resource.Url = resource.Url?.Trim();
            var previous = existing?.PublicationResources.FirstOrDefault(x => x.Id == resource.Id);
            if (resource.PdfFile.BlobId.HasValue && (resource.PdfFile.Id != previous?.PdfFile.Id || resource.PdfFile.BlobId != previous?.PdfFile.BlobId))
            {
                var saved = await fileService.SavePdf(new(sessionId, resource.PdfFile));
                if (!saved.Ok) { response.AddErrors(saved.Errors); return false; }
                resource.PdfFile = saved.Value!;
            }
            if (resource.ImageFile.BlobId.HasValue && (resource.ImageFile.Id != previous?.ImageFile.Id || resource.ImageFile.BlobId != previous?.ImageFile.BlobId))
            {
                var saved = await fileService.SaveImage(new(sessionId, resource.ImageFile));
                if (!saved.Ok) { response.AddErrors(saved.Errors); return false; }
                resource.ImageFile = saved.Value!;
            }
        }
        return true;
    }
}