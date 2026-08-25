create proc [EducationPublisher].[ForumLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ForumId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Content].[ForumLicense-Active] with (updlock, holdlock)
    where Id != @Id
        and ForumId = @ForumId
        and LicenseId = (select LicenseId from [Content].[ForumLicense-Active] where Id = @Id)
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ForumId = @ForumId
from [Content].[ForumLicense] baseTable
    inner join [Content].[ForumLicense-Active] forumLicense on forumLicense.Id = baseTable.Id
    inner join [Content].[Forum-Active] forum on forumLicense.ForumId = forum.Id
    inner join [Framework].[License-Active] license on forumLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
where baseTable.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction