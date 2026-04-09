create proc [ContentDisplay].[ElementLinkFollowedInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@Url nvarchar(250)
) as

declare @id uniqueidentifier = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[ElementLinkFollowed] (
     Id
    ,ElementId
    ,[Url]
    ,Followed
    ,FollowedBy
)
values (
     @id
    ,@ElementId
    ,@Url
    ,@now
    ,@SessionId
)