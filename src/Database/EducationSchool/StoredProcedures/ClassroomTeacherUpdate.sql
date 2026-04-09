create proc [EducationSchool].[ClassroomTeacherUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ClassroomId uniqueidentifier
    ,@SchoolContactId uniqueidentifier
) as
begin transaction

    declare @now datetimeoffset = sysdatetimeoffset()

    update [Education].[ClassroomTeacher]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,ClassroomId = @ClassroomId
        ,SchoolContactId = @SchoolContactId
    where Id = @Id

commit transaction