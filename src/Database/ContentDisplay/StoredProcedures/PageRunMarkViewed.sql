create proc [ContentDisplay].[PageRunMarkViewed] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

if @Id is null or @Id = '00000000-0000-0000-0000-000000000000'
    return

declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[PageViewed] (
    Id
    ,PageId
    ,Updated
    ,UpdatedBy
)
values (
    newid()
    ,@Id
    ,@now
    ,@SessionId
)