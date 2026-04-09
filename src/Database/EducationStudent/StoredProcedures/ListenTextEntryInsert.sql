create proc [EducationStudent].[ListenTextEntryInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ListenTextEntry] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,QuestionId
    ,Text
)
values (
     @Id
    ,@SessionId
    ,@AssignmentId
    ,@QuestionId
    ,@Text
)