create proc [EducationPublisher].[BlogLicenseDelete] (
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
from [Content].[BlogLicense] baseTable
    inner join [Content].[BlogLicense-Active] blogLicense on blogLicense.Id = baseTable.Id
    inner join [Content].[Blog-Active] blog on blogLicense.BlogId = blog.Id
    inner join [Framework].[License-Active] license on blogLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
where baseTable.Id = @Id
    and license.OwnerId = @organizationId
    and portal.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction