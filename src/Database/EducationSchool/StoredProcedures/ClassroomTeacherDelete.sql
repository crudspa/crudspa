create proc [EducationSchool].[ClassroomTeacherDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    update [Education].[ClassroomTeacher]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where Id = @Id

commit transaction