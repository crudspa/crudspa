create proc [FrameworkCore].[LinkFollowedInsert] (
     @SessionId uniqueidentifier
    ,@Url nvarchar(250)
) as

declare @id uniqueidentifier = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[LinkFollowed] (
     Id
    ,[Url]
    ,Followed
    ,FollowedBy
)
values (
     @id
    ,@Url
    ,@now
    ,@SessionId
)