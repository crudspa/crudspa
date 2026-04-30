create view [Content].[AnswerChoiceReply-Active] as

select answerChoiceReply.Id as Id
    ,answerChoiceReply.QuestionReplyId as QuestionReplyId
    ,answerChoiceReply.ChoiceId as ChoiceId
from [Content].[AnswerChoiceReply] answerChoiceReply
where 1=1
    and answerChoiceReply.IsDeleted = 0
    and answerChoiceReply.VersionOf = answerChoiceReply.Id