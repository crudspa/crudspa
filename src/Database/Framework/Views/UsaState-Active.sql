create view [Framework].[UsaState-Active] as

select usaState.Id as Id
    ,usaState.Abbreviation as Abbreviation
    ,usaState.Name as Name
from [Framework].[UsaState] usaState
where 1=1
    and usaState.IsDeleted = 0