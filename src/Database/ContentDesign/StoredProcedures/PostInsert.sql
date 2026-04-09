create proc [ContentDesign].[PostInsert] (
     @SessionId uniqueidentifier
    ,@BlogId uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@Author nvarchar(150)
    ,@Published date
    ,@Revised date
    ,@CommentRule int
    ,@PageTypeId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @pageId uniqueidentifier = newid()

insert [Content].[Page] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,StatusId
    ,TypeId
    ,Ordinal
)
values (
     @pageId
    ,@pageId
    ,@now
    ,@SessionId
    ,'Post'
    ,@ContentStatusComplete
    ,@PageTypeId
    ,0
)

insert [Content].[Post] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BlogId
    ,Title
    ,StatusId
    ,Author
    ,Published
    ,Revised
    ,CommentRule
    ,PageId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@BlogId
    ,@Title
    ,@StatusId
    ,@Author
    ,@Published
    ,@Revised
    ,@CommentRule
    ,@pageId
)

if not exists (
    select 1
    from [Content].[Post-Active] post
        inner join [Content].[Blog-Active] blog on post.BlogId = blog.Id
        inner join [Framework].[Portal-Active] portal on blog.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where post.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction