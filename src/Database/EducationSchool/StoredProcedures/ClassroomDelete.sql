create proc [EducationSchool].[ClassroomDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @organizationId uniqueidentifier = (select top 1 OrganizationId from [Education].[Classroom] where Id = @Id)

begin transaction

    update [Education].[Classroom]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where Id = @Id

    update [Framework].[Organization]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where Id = @organizationId

commit transaction