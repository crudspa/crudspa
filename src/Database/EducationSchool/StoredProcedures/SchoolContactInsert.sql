create proc [EducationSchool].[SchoolContactInsert] (
     @SessionId uniqueidentifier
    ,@TitleId uniqueidentifier
    ,@TestAccount bit
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Id uniqueidentifier output
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

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[SchoolContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TitleId
    ,TestAccount
    ,ContactId
    ,UserId
    ,SchoolId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@TitleId
    ,@TestAccount
    ,@ContactId
    ,@UserId
    ,@schoolId
)

if not exists (
    select 1
    from [Education].[SchoolContact-Active] schoolContact
        inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    where schoolContact.Id = @Id
        and school.OrganizationId = @organizationId
        and school.Id = @schoolId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction