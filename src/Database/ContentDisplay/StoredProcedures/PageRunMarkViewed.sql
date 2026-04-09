create proc [ContentDisplay].[PageRunMarkViewed] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

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