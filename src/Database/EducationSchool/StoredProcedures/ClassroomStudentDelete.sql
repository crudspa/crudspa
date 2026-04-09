create proc [EducationSchool].[ClassroomStudentDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    update [Education].[ClassroomStudent]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where Id = @Id

commit transaction