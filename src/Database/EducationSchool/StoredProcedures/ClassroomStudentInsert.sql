create proc [EducationSchool].[ClassroomStudentInsert] (
     @SessionId uniqueidentifier
    ,@ClassroomId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    if not exists (select 1 from [Education].[ClassroomStudent-Active] where ClassroomId = @ClassroomId and StudentId = @StudentId)
    begin
        insert [Education].[ClassroomStudent] (
            Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,ClassroomId
            ,StudentId
        )
        values (
            @Id
            ,@Id
            ,@now
            ,@SessionId
            ,@ClassroomId
            ,@StudentId
        )
    end

commit transaction