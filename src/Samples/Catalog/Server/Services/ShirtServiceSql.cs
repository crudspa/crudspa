namespace Crudspa.Samples.Catalog.Server.Services;

public class ShirtServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IShirtService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Shirt>>> Search(Request<ShirtSearch> request)
    {
        return await wrappers.Try<IList<Shirt>>(request, async response =>
        {
            var shirts = await ShirtSelectWhere.Execute(Connection, request.SessionId, request.Value);

            return shirts;
        });
    }

    public async Task<Response<Shirt?>> Fetch(Request<Shirt> request)
    {
        return await wrappers.Try<Shirt?>(request, async response =>
        {
            var shirt = await ShirtSelect.Execute(Connection, request.SessionId, request.Value);

            return shirt;
        });
    }

    public async Task<Response<Shirt?>> Add(Request<Shirt> request)
    {
        return await wrappers.Validate<Shirt?, Shirt>(request, async response =>
        {
            var shirt = request.Value;

            var heroImageFileResponse = await fileService.SaveImage(new(request.SessionId, shirt.HeroImageFile));
                if (!heroImageFileResponse.Ok)
                {
                    response.AddErrors(heroImageFileResponse.Errors);
                    return null;
                }

                shirt.HeroImageFile.Id = heroImageFileResponse.Value.Id;

            var guidePdfFileResponse = await fileService.SavePdf(new(request.SessionId, shirt.GuidePdfFile));
                if (!guidePdfFileResponse.Ok)
                {
                    response.AddErrors(guidePdfFileResponse.Errors);
                    return null;
                }

                shirt.GuidePdfFile.Id = guidePdfFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ShirtInsert.Execute(connection, transaction, request.SessionId, shirt);

                return new Shirt
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Shirt> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var shirt = request.Value;

            var existing = await ShirtSelect.Execute(Connection, request.SessionId, shirt);

            var heroImageFileResponse = await fileService.SaveImage(new(request.SessionId, shirt.HeroImageFile), existing?.HeroImageFile);
            if (!heroImageFileResponse.Ok)
            {
                response.AddErrors(heroImageFileResponse.Errors);
                return;
            }

            shirt.HeroImageFile.Id = heroImageFileResponse.Value.Id;

            var guidePdfFileResponse = await fileService.SavePdf(new(request.SessionId, shirt.GuidePdfFile), existing?.GuidePdfFile);
            if (!guidePdfFileResponse.Ok)
            {
                response.AddErrors(guidePdfFileResponse.Errors);
                return;
            }

            shirt.GuidePdfFile.Id = guidePdfFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtUpdate.Execute(connection, transaction, request.SessionId, shirt);
            });
        });
    }

    public async Task<Response> Remove(Request<Shirt> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var shirt = request.Value;
            var existing = await ShirtSelect.Execute(Connection, request.SessionId, shirt);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtDelete.Execute(connection, transaction, request.SessionId, shirt);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchBrandNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BrandSelectOrderables.Execute(Connection, request.SessionId));
    }
}