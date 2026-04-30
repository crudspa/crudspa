create trigger [Content].[AnswerChoiceReplyTrigger] on [Content].[AnswerChoiceReply]
    for update
as

insert [Content].[AnswerChoiceReply] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionReplyId
    ,ChoiceId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionReplyId
    ,deleted.ChoiceId
from deleted