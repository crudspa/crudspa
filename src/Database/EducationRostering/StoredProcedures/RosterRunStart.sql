create proc [EducationRostering].[RosterRunStart] (
     @RosterSourceId uniqueidentifier
    ,@Kind nvarchar(25)
) as

set xact_abort on

if not exists (
    select 1
    from [Education].[RosterSource] rosterSource with (updlock, holdlock)
    where rosterSource.Id = @RosterSourceId
        and rosterSource.VersionOf = rosterSource.Id
        and rosterSource.IsDeleted = 0
        and rosterSource.Mode <> N'disabled'
)
    throw 50000, 'Roster source is unavailable or disabled.', 1

if exists (
    select 1
    from [Education].[RosterRun] rosterRun with (updlock, holdlock)
    where rosterRun.RosterSourceId = @RosterSourceId
        and rosterRun.Status = N'started'
)
    throw 50000, 'A roster run is already in progress for this source.', 1

declare @id uniqueidentifier = newid()

insert [Education].[RosterRun] (
     Id
    ,RosterSourceId
    ,Kind
    ,Status
)
values (
     @id
    ,@RosterSourceId
    ,@Kind
    ,N'started'
)

exec [EducationRostering].[RosterRunSelect] @RosterRunId = @id