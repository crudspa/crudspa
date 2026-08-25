create proc [EducationPublisher].[ForumLicenseDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
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