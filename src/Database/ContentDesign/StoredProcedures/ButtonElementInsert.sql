create proc [ContentDesign].[ButtonElementInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@ButtonId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[ButtonElement] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,ButtonId
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@ButtonId
)