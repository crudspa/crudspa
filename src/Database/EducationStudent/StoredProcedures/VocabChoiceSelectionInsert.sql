create proc [EducationStudent].[VocabChoiceSelectionInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ChoiceId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[VocabChoiceSelection] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,ChoiceId
    ,Made
)
values (
     newid()
    ,@SessionId
    ,@AssignmentId
    ,@ChoiceId
    ,@now
)