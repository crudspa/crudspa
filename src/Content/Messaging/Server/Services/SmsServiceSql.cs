namespace Crudspa.Content.Messaging.Server.Services;

public class SmsServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : ISmsService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Sms>>> SearchForMembership(Request<SmsSearch> request)
    {
        return await wrappers.Try<IList<Sms>>(request, async response =>
        {
            var sms = await SmsSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return sms;
        });
    }

    public async Task<Response<IList<Sms>>> SearchForPortal(Request<SmsSearch> request)
    {
        return await wrappers.Try<IList<Sms>>(request, async response =>
        {
            var sms = await SmsSelectWhereForPortal.Execute(Connection, request.SessionId, request.Value);

            return sms;
        });
    }

    public async Task<Response<Sms?>> Fetch(Request<Sms> request)
    {
        return await wrappers.Try<Sms?>(request, async response =>
        {
            var sms = await SmsSelect.Execute(Connection, request.SessionId, request.Value);

            return sms;
        });
    }

    public async Task<Response<Sms?>> Add(Request<Sms> request)
    {
        return await wrappers.Validate<Sms?, Sms>(request, async response =>
        {
            var sms = request.Value;

            foreach (var smsAttachment in sms.SmsAttachments)
            {
                var smsAttachmentImageFileResponse = await fileService.SaveImage(new(request.SessionId, smsAttachment.ImageFile), smsAttachment.ImageFile.Id);
                if (!smsAttachmentImageFileResponse.Ok)
                {
                    response.AddErrors(smsAttachmentImageFileResponse.Errors);
                    return null;
                }

                if (smsAttachmentImageFileResponse.Value is not null) smsAttachment.ImageFile = smsAttachmentImageFileResponse.Value;
            }

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await SmsInsert.Execute(connection, transaction, request.SessionId, sms);

                foreach (var smsAttachment in sms.SmsAttachments)
                {
                    smsAttachment.SmsId = id;
                    await SmsAttachmentInsertByBatch.Execute(connection, transaction, request.SessionId, smsAttachment);
                }

                return new Sms
                {
                    Id = id,
                    MembershipId = sms.MembershipId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Sms> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var sms = request.Value;

            var existing = await SmsSelect.Execute(Connection, request.SessionId, sms);

            foreach (var smsAttachment in sms.SmsAttachments)
            {
                var existingSmsAttachment = existing?.SmsAttachments.FirstOrDefault(x => x.Id.Equals(smsAttachment.Id));

                if (smsAttachment.ImageFile is not null)
                {
                    var smsAttachmentImageFileResponse = await fileService.SaveImage(new(request.SessionId, smsAttachment.ImageFile), existingSmsAttachment?.ImageFile?.Id);
                    if (!smsAttachmentImageFileResponse.Ok)
                    {
                        response.AddErrors(smsAttachmentImageFileResponse.Errors);
                        return;
                    }

                    if (smsAttachmentImageFileResponse.Value is not null) smsAttachment.ImageFile = smsAttachmentImageFileResponse.Value;
                }
            }

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await SmsUpdate.Execute(connection, transaction, request.SessionId, sms);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.SmsAttachments,
                    sms.SmsAttachments,
                    SmsAttachmentInsertByBatch.Execute,
                    SmsAttachmentUpdateByBatch.Execute,
                    SmsAttachmentDeleteByBatch.Execute);

                sms.SmsAttachments.EnsureOrder();
                await SmsAttachmentUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, sms.SmsAttachments);
            });
        });
    }

    public async Task<Response> Remove(Request<Sms> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var sms = request.Value;
            var existing = await SmsSelect.Execute(Connection, request.SessionId, sms);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var smsAttachment in existing.SmsAttachments)
                    await SmsAttachmentDeleteByBatch.Execute(connection, transaction, request.SessionId, smsAttachment);

                await SmsDelete.Execute(connection, transaction, request.SessionId, sms);
            });
        });
    }

    public async Task<Response<IList<SmsTemplateFull>>> FetchSmsTemplates(Request<Portal> request)
    {
        return await wrappers.Try<IList<SmsTemplateFull>>(request, async response =>
            await SmsTemplateSelectFull.Execute(Connection, request.SessionId, request.Value.Id));
    }
}