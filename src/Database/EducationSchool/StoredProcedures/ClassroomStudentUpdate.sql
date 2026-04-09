create proc [EducationSchool].[ClassroomStudentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ClassroomId uniqueidentifier
    ,@StudentId uniqueidentifier
) as
begin transaction

    declare @now datetimeoffset = sysdatetimeoffset()

    update [Education].[ClassroomStudent]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,ClassroomId = @ClassroomId
        ,StudentId = @StudentId
    where Id = @Id

commit transaction