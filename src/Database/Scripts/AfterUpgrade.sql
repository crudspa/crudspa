declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset = sysdatetimeoffset()
declare @id uniqueidentifier

declare @major tinyint
declare @minor tinyint
declare @build tinyint
declare @revision tinyint
declare @notes nvarchar(max)

select top 1
     @major = Major
    ,@minor = Minor
    ,@build = Build
    ,@revision = Revision
from [Framework].[Version]
order by Major desc, Minor desc, Build desc, Revision desc

if (@major is null)
begin
    set @major = 1
    set @minor = 0
    set @build = 100
    set @revision = 0
    set @notes = 'Initial deployment.'

    insert [Framework].[Version] (Id, Created, Major, Minor, Build, Revision, Notes)
    values (newid(), @now, 1, 0, @build, 0, @notes)
end

-- Apply updates in order, incrementing the version number as you go

if (@major = 1 and @minor = 0 and @build = 100 and @revision = 0)
begin
    set @build = 101
    set @notes = 'Sample upgrade script.'

    print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + concat('Upgrading to ', @major, '.', @minor, '.', @build, '.', @revision);
    print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + @notes

    insert [Framework].[Version] (Id, Created, Major, Minor, Build, Revision, Notes)
    values (newid(), @now, 1, 0, @build, 0, @notes)
end

print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Updating statistics...';
exec sp_updatestats
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Statistics updated.';