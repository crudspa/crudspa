using Crudspa.Content.Display.Shared.Contracts.Data;
using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Publisher.Server.Services;

public class ElementRepositoryActivity(IServerConfigService configService, IFileService fileService, IActivityService activityService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var activityElementId = Guid.NewGuid();
        var elementId = Guid.NewGuid();

        var sectionElement = new SectionElement
        {
            Element = new()
            {
                Id = elementId,
                SectionId = sectionId,
                TypeId = elementType.Id,
                ElementType = elementType,
                Ordinal = ordinal,
                Item = new()
                {
                    BasisId = BasisIds.Auto,
                    Grow = "0",
                    Shrink = "1",
                    AlignSelfId = AlignSelfIds.Auto,
                },
            },
        };

        sectionElement.SetConfig(new ActivityElement
        {
            Id = activityElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var activityElement = element.RequireConfig<ActivityElement>();

        return await ErrorsEx.Validate(async errors =>
        {
            errors.AddRange(element.Element.Validate());

            var activity = activityElement.Activity!;
            var activityType = (await ActivityTypeSelectFull.Execute(connection)).First(x => x.Id.Equals(activity.ActivityTypeId));

            ActivityServiceSql.Sanitize(activity, activityType);
            ActivityServiceSql.ValidateDto(activity, errors);
            ActivityServiceSql.ValidateUsingMetadata(activity, activityType, errors);
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var activityElement = element.RequireConfig<ActivityElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        activityElement.ElementId = elementId;

        var activityResponse = await activityService.Add(new(sessionId, activityElement.Activity!));

        if (!activityResponse.Ok)
            throw new("Call to IActivityService.Add() failed.");

        activityElement.ActivityId = activityResponse.Value.Id;

        await ActivityElementInsert.Execute(connection, transaction, sessionId, activityElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var activityElement = element.RequireConfig<ActivityElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        element.Box.BackgroundImageFile.Id = (await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile)).Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var activityResponse = await activityService.Save(new(sessionId, activityElement.Activity!));

        if (!activityResponse.Ok)
            throw new("Call to IActivityService.Save() failed.");

        await ActivityElementUpdate.Execute(connection, transaction, sessionId, activityElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var activityElement = element.RequireConfig<ActivityElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await ActivityDelete.Execute(connection, transaction, sessionId, new() { Id = activityElement.ActivityId });
        await ActivityElementDelete.Execute(connection, transaction, sessionId, activityElement);
    }
}