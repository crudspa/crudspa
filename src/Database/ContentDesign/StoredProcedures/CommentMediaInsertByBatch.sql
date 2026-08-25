create proc [ContentDesign].[CommentMediaInsertByBatch] (
     @SessionId uniqueidentifier
    ,@CommentId uniqueidentifier
    ,@Type int
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId and session.Ended is null
)

if @organizationId is null or not exists (
    select 1
    from [Content].[Comment-Active] comment
    where comment.Id = @CommentId
        and (
            exists (
                select 1
                from [Content].[Post-Active] post
                    inner join [Content].[Blog-Active] blog on blog.Id = post.BlogId
                    inner join [Framework].[Portal-Active] portal on portal.Id = blog.PortalId
                where post.Id = comment.PostId and portal.OwnerId = @organizationId
            )
            or exists (
                select 1
                from [Content].[Thread-Active] thread
                    inner join [Content].[Forum-Active] forum on forum.Id = thread.ForumId
                    inner join [Framework].[Portal-Active] portal on portal.Id = forum.PortalId
                where (thread.Id = comment.ThreadId or thread.CommentId = comment.Id)
                    and portal.OwnerId = @organizationId
            )
        )
)
begin
    rollback transaction
    raiserror('Comment media tenancy check failed', 16, 1)
    return
end

insert [Content].[CommentMedia] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,CommentId
    ,Type
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@CommentId
    ,@Type
    ,@AudioId
    ,@ImageId
    ,@PdfId
    ,@VideoId
    ,@Ordinal
)

commit transaction