create proc [EducationCommon].[ActivityAssignmentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Started datetimeoffset
    ,@Finished datetimeoffset
    ,@StatusId uniqueidentifier
) as
begin transaction

    declare @now datetimeoffset = sysdatetimeoffset()

    update [Education].[ActivityAssignment]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,Started = @Started
        ,Finished = @Finished
        ,StatusId = @StatusId
    where Id = @Id

commit transaction