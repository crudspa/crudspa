create view [Content].[CampaignSchedule-Active] as

select campaignSchedule.Id as Id
    ,campaignSchedule.ActivationScopeId as ActivationScopeId
    ,campaignSchedule.GradeId as GradeId
    ,campaignSchedule.LessonStart as LessonStart
    ,campaignSchedule.LessonStartOverridden as LessonStartOverridden
    ,campaignSchedule.AssessmentStart as AssessmentStart
    ,campaignSchedule.AssessmentStartOverridden as AssessmentStartOverridden
from [Content].[CampaignSchedule] campaignSchedule
where 1=1
    and campaignSchedule.IsDeleted = 0
    and campaignSchedule.VersionOf = campaignSchedule.Id