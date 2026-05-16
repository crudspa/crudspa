create trigger [Content].[SurveyReplyTrigger] on [Content].[SurveyReply]
    for update
as

insert [Content].[SurveyReply] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SurveyId
    ,BinderId
    ,ContactId
    ,Started
    ,Completed
    ,Terminated
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SurveyId
    ,deleted.BinderId
    ,deleted.ContactId
    ,deleted.Started
    ,deleted.Completed
    ,deleted.Terminated
from deleted