create proc [FrameworkCore].[UsaStateSelectNames] as

select
    usaState.Id
    ,usaState.Abbreviation
from [Framework].[UsaState-Active] usaState
order by usaState.Abbreviation