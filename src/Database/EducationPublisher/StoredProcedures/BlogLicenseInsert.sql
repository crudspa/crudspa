create proc [EducationPublisher].[BlogLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@BlogId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Content].[BlogLicense-Active] with (updlock, holdlock)
    where BlogId = @BlogId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Content].[BlogLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,BlogId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@BlogId
)

if not exists (
    select 1
    from [Content].[BlogLicense-Active] blogLicense
        inner join [Content].[Blog-Active] blog on blogLicense.BlogId = blog.Id
        inner join [Framework].[License-Active] license on blogLicense.LicenseId = license.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
    where blogLicense.Id = @Id
        and license.OwnerId = @organizationId
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction