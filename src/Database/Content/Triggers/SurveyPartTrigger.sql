create trigger [Content].[SurveyPartTrigger] on [Content].[SurveyPart]
    for update
as

insert [Content].[SurveyPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SurveyId
    ,Title
    ,Instructions
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SurveyId
    ,deleted.Title
    ,deleted.Instructions
    ,deleted.Ordinal
from deleted