create proc [EducationStudent].[ReadTextEntryInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ReadTextEntry] (
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