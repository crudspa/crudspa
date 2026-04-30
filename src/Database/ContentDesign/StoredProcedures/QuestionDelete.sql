create proc [ContentDesign].[QuestionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update choice
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[OptionsAnswerChoice] choice
    inner join [Content].[OptionsAnswer-Active] answer on choice.OptionsAnswerId = answer.Id
where answer.QuestionId = @Id
    and choice.IsDeleted = 0

update [Content].[BooleanAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[ContactAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[DateAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[FileAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[NumberAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[OptionsAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[ScaleAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[TextAnswer]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where QuestionId = @Id
    and IsDeleted = 0

update [Content].[Question]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id

commit transaction