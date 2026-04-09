create proc [EducationStudent].[AppSurveyResponseInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentBatchId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@AnswerId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[AppSurveyResponse] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,AssignmentBatchId
    ,QuestionId
    ,AnswerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AssignmentBatchId
    ,@QuestionId
    ,@AnswerId
)