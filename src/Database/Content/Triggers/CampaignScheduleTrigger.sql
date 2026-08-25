create trigger [Content].[CampaignScheduleTrigger] on [Content].[CampaignSchedule]
    for update
as

insert [Content].[CampaignSchedule] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ActivationScopeId
    ,GradeId
    ,LessonStart
    ,LessonStartOverridden
    ,AssessmentStart
    ,AssessmentStartOverridden
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ActivationScopeId
    ,deleted.GradeId
    ,deleted.LessonStart
    ,deleted.LessonStartOverridden
    ,deleted.AssessmentStart
    ,deleted.AssessmentStartOverridden
from deleted