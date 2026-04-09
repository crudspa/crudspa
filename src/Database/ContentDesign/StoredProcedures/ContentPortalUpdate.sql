create proc [ContentDesign].[ContentPortalUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@MaxWidth nvarchar(10)
    ,@BrandingImageId uniqueidentifier
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

if not exists (
    select 1
    from [Content].[ContentPortal-Active] contentPortal
        inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where contentPortal.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update contentPortal
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,MaxWidth = @MaxWidth
    ,BrandingImageId = @BrandingImageId
from [Content].[ContentPortal] contentPortal
where contentPortal.Id = @Id

commit transaction