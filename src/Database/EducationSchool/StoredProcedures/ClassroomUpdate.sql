create proc [EducationSchool].[ClassroomUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@TypeId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@SmallClassroom bit
) as

begin transaction

    declare @now datetimeoffset = sysdatetimeoffset()
    declare @organizationId uniqueidentifier = (select top 1 OrganizationId from [Education].[Classroom] where Id = @Id)

    update [Framework].[Organization]
    set
        Id = @organizationId
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,Name = @Name
    where Id = @organizationId

    update [Education].[Classroom]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,TypeId = @TypeId
        ,GradeId = @GradeId
        ,SmallClassroom = @SmallClassroom
    where Id = @Id

commit transaction