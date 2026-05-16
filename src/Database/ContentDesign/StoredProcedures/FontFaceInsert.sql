create proc [ContentDesign].[FontFaceInsert] (
     @SessionId uniqueidentifier
    ,@FontId uniqueidentifier
    ,@FileId uniqueidentifier
    ,@Style nvarchar(10)
    ,@WeightMin int
    ,@WeightMax int
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

insert [Content].[FontFace] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,FontId
    ,FileId
    ,Style
    ,WeightMin
    ,WeightMax
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@FontId
    ,@FileId
    ,@Style
    ,@WeightMin
    ,@WeightMax
)

if not exists (
    select 1
    from [Content].[FontFace-Active] fontFace
        inner join [Content].[Font-Active] font on fontFace.FontId = font.Id
        inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
        inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where fontFace.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @contentPortalId uniqueidentifier = (
    select top 1 font.ContentPortalId
    from [Content].[Font-Active] font
    where font.Id = @FontId
)

update [Content].[ContentPortal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,StyleRevision = StyleRevision + 1
where Id = @contentPortalId

commit transaction