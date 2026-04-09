create proc [EducationStudent].[ReadChoiceSelectionInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ChoiceId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ReadChoiceSelection] (
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