create proc [FrameworkJobs].[JobSelectWhere] (
     @PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Types Framework.IdList readonly
    ,@AddedStart datetimeoffset(7)
    ,@AddedEnd datetimeoffset(7)
    ,@Status Framework.IdList readonly
    ,@Devices Framework.IdList readonly
    ,@Schedules Framework.IdList readonly
) as

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @typesCount int = (select count(1) from @Types)
declare @statusCount int = (select count(1) from @Status)
declare @devicesCount int = (select count(1) from @Devices)
declare @schedulesCount int = (select count(1) from @Schedules)

;with JobCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Added' and @SortAscending = 1)
                    then job.Added
                end asc,
                case when (@SortField = 'Added' and @SortAscending = 0)
                    then job.Added
                end desc,
                case when (@SortField = 'Started' and @SortAscending = 1)
                    then job.Started
                end asc,
                case when (@SortField = 'Started' and @SortAscending = 0)
                    then job.Started
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,job.Id
    from [Framework].[Job-Active] job
    where 1 = 1
        and (@SearchText is null
            or job.Description like '%' + @SearchText + '%'
        )
        and (@typesCount = 0 or job.TypeId in (select Id from @Types))
        and (@AddedStart is null or job.Added >= @AddedStart)
        and (@AddedEnd is null or job.Added < @AddedEnd)
        and (@statusCount = 0 or job.StatusId in (select Id from @Status))
        and (@devicesCount = 0 or job.DeviceId in (select Id from @Devices))
        and (@schedulesCount = 0 or job.ScheduleId in (select Id from @Schedules))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,job.Id
    ,job.TypeId
    ,job.Description
    ,job.Added
    ,job.Started
    ,job.Ended
    ,job.StatusId
    ,job.DeviceId
    ,job.ScheduleId
     ,type.Name as TypeName
    ,status.Name as StatusName
    ,status.CssClass as StatusCssClass
    ,device.Name as DeviceName
    ,schedule.Name as ScheduleName
from JobCte cte
    inner join [Framework].[Job-Active] job on cte.Id = job.Id
    inner join [Framework].[JobType-Active] type on job.TypeId = type.Id
    inner join [Framework].[JobStatus-Active] status on job.StatusId = status.Id
    left join [Framework].[Device-Active] device on job.DeviceId = device.Id
    left join [Framework].[JobSchedule-Active] schedule on job.ScheduleId = schedule.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber