create proc [EducationSchool].[ClassroomTeacherInsert] (
     @SessionId uniqueidentifier
    ,@ClassroomId uniqueidentifier
    ,@SchoolContactId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    if not exists (select 1 from [Education].[ClassroomTeacher-Active] where ClassroomId = @ClassroomId and SchoolContactId = @SchoolContactId)
    begin
        insert [Education].[ClassroomTeacher] (
            Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,ClassroomId
            ,SchoolContactId
        )
        values (
            @Id
            ,@Id
            ,@now
            ,@SessionId
            ,@ClassroomId
            ,@SchoolContactId
        )
    end

commit transaction