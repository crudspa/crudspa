create proc [EducationSchool].[SchoolContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TitleId uniqueidentifier
    ,@TestAccount bit
    ,@UserId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TitleId = @TitleId
    ,TestAccount = @TestAccount
    ,UserId = @UserId
from [Education].[SchoolContact] baseTable
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = baseTable.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
where baseTable.Id = @Id
    and school.OrganizationId = @organizationId
    and school.Id = @schoolId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction