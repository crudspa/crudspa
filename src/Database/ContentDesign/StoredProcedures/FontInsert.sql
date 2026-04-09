create proc [ContentDesign].[FontInsert] (
     @SessionId uniqueidentifier
    ,@ContentPortalId uniqueidentifier
    ,@Name nvarchar(75)
    ,@FileId uniqueidentifier
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

insert [Content].[Font] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContentPortalId
    ,Name
    ,FileId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContentPortalId
    ,@Name
    ,@FileId
)

if not exists (
    select 1
    from [Content].[Font-Active] font
        inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
        inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where font.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update [Content].[ContentPortal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,StyleRevision = StyleRevision + 1
where Id = @ContentPortalId

commit transaction