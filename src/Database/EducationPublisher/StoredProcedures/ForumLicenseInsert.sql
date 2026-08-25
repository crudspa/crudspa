create proc [EducationPublisher].[ForumLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@ForumId uniqueidentifier
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
    from [Content].[ForumLicense-Active] with (updlock, holdlock)
    where ForumId = @ForumId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Content].[ForumLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,ForumId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@ForumId
)

if not exists (
    select 1
    from [Content].[ForumLicense-Active] forumLicense
        inner join [Content].[Forum-Active] forum on forumLicense.ForumId = forum.Id
        inner join [Framework].[License-Active] license on forumLicense.LicenseId = license.Id
        inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
    where forumLicense.Id = @Id
        and portal.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction