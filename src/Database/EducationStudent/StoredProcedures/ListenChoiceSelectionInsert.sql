create proc [EducationStudent].[ListenChoiceSelectionInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ChoiceId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ListenChoiceSelection] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,ChoiceId
    ,Made
)
values (
     @Id
    ,@SessionId
    ,@AssignmentId
    ,@ChoiceId
    ,@now
)