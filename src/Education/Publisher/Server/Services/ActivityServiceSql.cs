namespace Crudspa.Education.Publisher.Server.Services;

public class ActivityServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IActivityService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Activity?>> Fetch(Request<Activity> request)
    {
        return await wrappers.Try<Activity?>(request, async response =>
        {
            var activity = await ActivitySelect.Execute(Connection, request.Value);
            return activity;
        });
    }

    public async Task<Response<Activity?>> Add(Request<Activity> request)
    {
        return await wrappers.Validate<Activity?, Activity>(request, async response =>
        {
            var activity = request.Value;
            var activityType = (await ActivityTypeSelectFull.Execute(Connection)).First(x => x.Id.Equals(activity.ActivityTypeId));

            Sanitize(activity, activityType);

            var errors = new List<Error>();

            ValidateDto(activity, errors);
            ValidateUsingMetadata(activity, activityType, errors);

            if (errors.HasItems())
            {
                response.AddErrors(errors);
                return null;
            }

            var contextAudioResponse = await fileService.SaveAudio(new(request.SessionId, activity.ContextAudioFile));
            if (!contextAudioResponse.Ok)
            {
                response.AddErrors(contextAudioResponse.Errors);
                return null;
            }

            activity.ContextAudioFile.Id = contextAudioResponse.Value.Id;

            var contextImageResponse = await fileService.SaveImage(new(request.SessionId, activity.ContextImageFile));
            if (!contextImageResponse.Ok)
            {
                response.AddErrors(contextImageResponse.Errors);
                return null;
            }

            activity.ContextImageFile.Id = contextImageResponse.Value.Id;

            foreach (var activityChoice in activity.ActivityChoices)
            {
                var audioResponse = await fileService.SaveAudio(new(request.SessionId, activityChoice.AudioFile!));
                if (!audioResponse.Ok)
                {
                    response.AddErrors(audioResponse.Errors);
                    return null;
                }

                activityChoice.AudioFile!.Id = audioResponse.Value.Id;

                var imageResponse = await fileService.SaveImage(new(request.SessionId, activityChoice.ImageFile!));
                if (!imageResponse.Ok)
                {
                    response.AddErrors(imageResponse.Errors);
                    return null;
                }

                activityChoice.ImageFile!.Id = imageResponse.Value.Id;
            }

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await ActivityInsert.Execute(connection, transaction, request.SessionId, activity);

                foreach (var activityChoice in activity.ActivityChoices)
                {
                    activityChoice.ActivityId = id;
                    await ActivityChoiceInsertByBatch.Execute(connection, transaction, request.SessionId, activityChoice);
                }

                return new Activity
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Activity> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var activity = request.Value;
            var activityType = (await ActivityTypeSelectFull.Execute(Connection)).First(x => x.Id.Equals(activity.ActivityTypeId));

            Sanitize(activity, activityType);

            var errors = new List<Error>();

            ValidateDto(activity, errors);
            ValidateUsingMetadata(activity, activityType, errors);

            if (errors.HasItems())
            {
                response.AddErrors(errors);
                return;
            }

            var existing = await ActivitySelect.Execute(Connection, activity);

            var contextAudioResponse = await fileService.SaveAudio(new(request.SessionId, activity.ContextAudioFile), existing?.ContextAudioFile);
            if (!contextAudioResponse.Ok)
            {
                response.AddErrors(contextAudioResponse.Errors);
                return;
            }

            activity.ContextAudioFile.Id = contextAudioResponse.Value.Id;

            var contextImageResponse = await fileService.SaveImage(new(request.SessionId, activity.ContextImageFile), existing?.ContextImageFile);
            if (!contextImageResponse.Ok)
            {
                response.AddErrors(contextImageResponse.Errors);
                return;
            }

            activity.ContextImageFile.Id = contextImageResponse.Value.Id;

            foreach (var activityChoice in activity.ActivityChoices)
            {
                var existingActivityChoice = existing?.ActivityChoices.FirstOrDefault(x => x.Id.Equals(activityChoice.Id));

                var audioResponse = await fileService.SaveAudio(new(request.SessionId, activityChoice.AudioFile!), existingActivityChoice?.AudioFile);
                if (!audioResponse.Ok)
                {
                    response.AddErrors(audioResponse.Errors);
                    return;
                }

                activityChoice.AudioFile!.Id = audioResponse.Value.Id;

                var imageResponse = await fileService.SaveImage(new(request.SessionId, activityChoice.ImageFile!), existingActivityChoice?.ImageFile);
                if (!imageResponse.Ok)
                {
                    response.AddErrors(imageResponse.Errors);
                    return;
                }

                activityChoice.ImageFile!.Id = imageResponse.Value.Id;
            }

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ActivityUpdate.Execute(connection, transaction, request.SessionId, activity);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.ActivityChoices,
                    activity.ActivityChoices,
                    ActivityChoiceInsertByBatch.Execute,
                    ActivityChoiceUpdateByBatch.Execute,
                    ActivityChoiceDeleteByBatch.Execute);

                activity.ActivityChoices.EnsureOrder();
                await ActivityChoiceUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, activity.ActivityChoices);
            });
        });
    }

    public async Task<Response> Remove(Request<Activity> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var activity = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ActivityDelete.Execute(connection, transaction, request.SessionId, activity);
            });
        });
    }

    #region Sanitization and Validation

    public static void Sanitize(Activity activity, ActivityTypeFull activityType)
    {
        if (activityType.SupportsContextText != true)
            activity.ContextText = null;

        if (activityType.SupportsContextAudio != true)
            activity.ContextAudioFile = new();

        if (activityType.SupportsExtraText != true)
            activity.ExtraText = null;

        if (activityType.SupportsChoices != true)
            activity.ActivityChoices = [];

        if (activity.ActivityChoices.HasItems())
        {
            if (activityType.RequiresAudioChoices != true && activityType.SupportsAudioChoices != true)
                foreach (var activityChoice in activity.ActivityChoices)
                    activityChoice.AudioFile = new();

            if (activityType.RequiresImageChoices != true && activityType.RequiresTextOrImageChoices != true)
                foreach (var activityChoice in activity.ActivityChoices)
                    activityChoice.ImageFile = new();

            if (activityType.RequiresTextChoices != true && activityType.RequiresLongerTextChoices != true && activityType.SupportsTextChoices != true)
                foreach (var activityChoice in activity.ActivityChoices)
                    activityChoice.Text = null;

            if (activityType.RequiresCorrectChoices != true)
                foreach (var activityChoice in activity.ActivityChoices)
                    activityChoice.IsCorrect = false;
        }
    }

    public static void ValidateDto(Activity activity, List<Error> errors)
    {
        errors.AddRange(activity.Validate());
    }

    public static void ValidateUsingMetadata(Activity activity, ActivityTypeFull activityType, List<Error> errors)
    {
        if (activityType.SupportsContextText == true
            && activityType.RequiresContextText == true
            && activity.ContextText.HasNothing())
        {
            errors.AddError("Context Text is required.");
        }

        if (activityType.RequiresContextImage == true
            && !activity.ContextImageFile.BlobId.HasValue)
        {
            errors.AddError("Context Image is required.");
        }

        if (activityType.SupportsContextAudio == true
            && activity.ContextAudioFile.Name.HasSomething()
            && !activity.ContextAudioFile.Name!.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
            && !activity.ContextAudioFile.Name.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase)
            && !activity.ContextAudioFile.Name.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            errors.AddError("Context Audio must be a WAV, MP3, or M4A file.");
        }

        if (activityType.SupportsContextAudio == true
            && activityType.RequiresContextAudio == true
            && !activity.ContextAudioFile.BlobId.HasValue)
        {
            errors.AddError("Context Audio is required.");
        }

        if (activityType.SupportsExtraText == true
            && activityType.RequiresExtraText == true
            && activity.ExtraText.HasNothing())
        {
            errors.AddError("Extra Text is required.");
        }

        if (activityType.SupportsChoices == true)
        {
            var choiceCount = activity.ActivityChoices?.Count ?? 0;
            var correctChoiceCount = activity.ActivityChoices?.Count(x => x.IsCorrect == true) ?? 0;

            var choiceRange = activityType.MinChoices == activityType.MaxChoices
                ? $"Exactly {activityType.MinChoices}"
                : $"Between {activityType.MinChoices}-{activityType.MaxChoices}";

            var choiceCorrectRange = activityType.MinCorrectChoices == activityType.MaxCorrectChoices
                ? $"Exactly {activityType.MinCorrectChoices}"
                : $"Between {activityType.MinCorrectChoices}-{activityType.MaxCorrectChoices}";

            if (choiceCount < activityType.MinChoices)
                errors.AddError($"{choiceRange} choices must be added.");

            if (choiceCount > activityType.MaxChoices)
                errors.AddError($"{choiceRange} choices must be added.");

            if (activityType.RequiresCorrectChoices == true)
            {
                if (correctChoiceCount < activityType.MinCorrectChoices)
                    errors.AddError($"{choiceCorrectRange} choices must be marked as Correct.");

                if (correctChoiceCount > activityType.MaxCorrectChoices)
                    errors.AddError($"{choiceCorrectRange} choices must be marked as Correct.");
            }

            if (activityType.RequiresAudioChoices == true
                && activity.ActivityChoices.HasAny(x => !x.AudioFile!.BlobId.HasValue))
                errors.AddError("All choices must include audio.");

            if (activityType.RequiresAudioChoices == true
                && activity.ActivityChoices.HasAny(x => x.AudioFile!.Name.HasSomething()
                    && !x.AudioFile.Name!.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                    && !x.AudioFile.Name.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase)
                    && !x.AudioFile.Name.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)))
            {
                errors.AddError("All audio files for choices must be WAV, MP3, or M4A format.");
            }

            if (activityType.RequiresImageChoices == true
                && activity.ActivityChoices.HasAny(x => !x.ImageFile!.BlobId.HasValue))
            {
                errors.AddError("All choices must include an image.");
            }

            if ((activityType.RequiresTextChoices == true || activityType.RequiresLongerTextChoices == true)
                && activity.ActivityChoices.HasAny(x => x.Text.HasNothing()))
            {
                errors.AddError("All choices must include text.");
            }

            if (activityType.RequiresTextOrImageChoices == true)
            {
                foreach (var choice in activity.ActivityChoices!)
                {
                    if (choice.Text.HasNothing() && !choice.ImageFile!.BlobId.HasValue)
                    {
                        errors.AddError("All choices must include either an image or text.");
                        break;
                    }
                }
            }
        }
    }

    #endregion
}