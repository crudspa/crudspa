create proc [ContentDesign].[ButtonElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ButtonId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Content].[ButtonElement]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ButtonId = @ButtonId
where Id = @Id