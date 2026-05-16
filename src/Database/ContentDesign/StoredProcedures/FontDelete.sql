create proc [ContentDesign].[FontDelete] (
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

update fontFaceBase
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[FontFace] fontFaceBase
    inner join [Content].[FontFace-Active] fontFace on fontFace.Id = fontFaceBase.Id
    inner join [Content].[Font-Active] font on fontFace.FontId = font.Id
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where fontFace.FontId = @Id
    and organization.Id = @organizationId

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Font] baseTable
    inner join [Content].[Font-Active] font on font.Id = baseTable.Id
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @contentPortalId uniqueidentifier = (select top 1 ContentPortalId from [Content].[Font] where Id = @Id)

update [Content].[ContentPortal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,StyleRevision = StyleRevision + 1
where Id = @contentPortalId

commit transaction