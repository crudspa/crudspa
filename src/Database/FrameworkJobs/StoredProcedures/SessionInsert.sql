create proc [FrameworkJobs].[SessionInsert] (
     @Id uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[Session] (
     Id
    ,Started
    ,PortalId
    ,UserId
    ,UserAdded
)
values (
     @Id
    ,@now
    ,@PortalId
    ,@UserId
    ,@now
)