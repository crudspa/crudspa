create proc [FrameworkCore].[ContentStatusSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     contentStatus.Id
    ,contentStatus.Name as Name
    ,contentStatus.Ordinal
from [Framework].[ContentStatus-Active] contentStatus
order by contentStatus.Ordinal