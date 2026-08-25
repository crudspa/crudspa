namespace Crudspa.Content.Display.Server.Services;

public class ElementProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer,
    ISessionLicenseResolver sessionLicenseResolver)
    : IElementProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ElementProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ElementProgress>>(request, async response =>
        {
            var progress = await ElementProgressSelectAll.Execute(Connection, request.SessionId);
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            foreach (var item in progress.ToList())
            {
                if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: item.ElementId))
                    progress.Remove(item);
            }

            return progress;
        });
    }

    public async Task<ElementProgress> Fetch(Request<Element> request)
    {
        var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
        return await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: request.Value.Id)
            ? await ElementProgressSelect.Execute(Connection, request.SessionId, request.Value.Id)
            : new();
    }

    public async Task<Response<QuestionReply?>> FetchQuestionReply(Request<Element> request)
    {
        return await wrappers.Try<QuestionReply?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: request.Value.Id)
                ? await QuestionReplySelectForElement.Execute(Connection, request.SessionId, request.Value.Id)
                : null;
        });
    }

    public async Task<Response> AddCompleted(Request<ElementCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: request.Value.ElementId))
            {
                response.AddError("Element is not accessible.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ElementCompletedInsert.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> AddLink(Request<ElementLink> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var elementLink = request.Value;
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: elementLink.ElementId))
            {
                response.AddError("Element is not accessible.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ElementLinkFollowedInsert.Execute(connection, transaction, request.SessionId, elementLink);
            });
        });
    }

    public async Task<Response> AddQuestionReply(Request<QuestionReply> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var reply = request.Value;

            if (!reply.QuestionId.HasValue)
            {
                response.AddError("Question is required.", nameof(reply.QuestionId));
                return;
            }

            if (!reply.ElementId.HasValue && !reply.SurveyReplyId.HasValue)
            {
                response.AddError("Element or survey reply is required.", nameof(reply.ElementId));
                return;
            }

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var accessible = reply.ElementId.HasValue
                ? await TrackContentIsAccessible.Execute(Connection, licenseIds, elementId: reply.ElementId)
                : await SurveyIsAccessible.Execute(Connection, licenseIds, surveyReplyId: reply.SurveyReplyId);

            if (!accessible)
            {
                response.AddError("Content is not accessible.");
                return;
            }

            var audioFileResponse = await fileService.SaveAudio(new(request.SessionId, reply.AudioFile));
            if (!audioFileResponse.Ok)
            {
                response.AddErrors(audioFileResponse.Errors);
                return;
            }
            reply.AudioId = audioFileResponse.Value?.Id;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, reply.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }
            reply.ImageId = imageFileResponse.Value?.Id;

            var pdfFileResponse = await fileService.SavePdf(new(request.SessionId, reply.PdfFile));
            if (!pdfFileResponse.Ok)
            {
                response.AddErrors(pdfFileResponse.Errors);
                return;
            }
            reply.PdfId = pdfFileResponse.Value?.Id;

            var videoFileResponse = await fileService.SaveVideo(new(request.SessionId, reply.VideoFile));
            if (!videoFileResponse.Ok)
            {
                response.AddErrors(videoFileResponse.Errors);
                return;
            }
            reply.VideoId = videoFileResponse.Value?.Id;

            reply.HtmlValue = htmlSanitizer.Sanitize(reply.HtmlValue);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                if (reply.Postal.HasContent())
                    reply.PostalId = await UsaPostalInsert.Execute(connection, transaction, request.SessionId, reply.Postal);

                reply.Id = await QuestionReplyInsert.Execute(connection, transaction, request.SessionId, reply);
                if (!reply.Id.HasValue)
                {
                    response.AddError("Unable to save the question reply.");
                    return;
                }

                foreach (var answerChoice in reply.AnswerChoices)
                {
                    answerChoice.QuestionReplyId = reply.Id;
                    await AnswerChoiceReplyInsertByBatch.Execute(connection, transaction, request.SessionId, answerChoice);
                }

                if (reply.ElementId.HasValue)
                {
                    await ElementCompletedInsert.Execute(connection, transaction, request.SessionId, new()
                    {
                        ElementId = reply.ElementId,
                        DeviceTimestamp = reply.Submitted ?? DateTimeOffset.Now,
                    });
                }
            });
        });
    }
}

internal static class QuestionReplyPostalEx
{
    public static Boolean HasContent(this UsaPostal postal) =>
        postal.RecipientName.HasSomething()
        || postal.BusinessName.HasSomething()
        || postal.StreetAddress.HasSomething()
        || postal.City.HasSomething()
        || postal.StateId.HasValue
        || postal.PostalCode.HasSomething();
}