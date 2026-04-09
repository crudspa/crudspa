create proc [FrameworkCore].[SessionInsert] (
     @Id uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[Session] (
     Id
    ,Started
    ,PortalId
)
values (
     @Id
    ,@now
    ,@PortalId
)