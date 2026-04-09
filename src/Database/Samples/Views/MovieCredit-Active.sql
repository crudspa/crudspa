create view [Samples].[MovieCredit-Active] as

select movieCredit.Id as Id
    ,movieCredit.MovieId as MovieId
    ,movieCredit.Name as Name
    ,movieCredit.Part as Part
    ,movieCredit.Billing as Billing
    ,movieCredit.Headliner as Headliner
    ,movieCredit.Ordinal as Ordinal
from [Samples].[MovieCredit] movieCredit
where 1=1
    and movieCredit.IsDeleted = 0
    and movieCredit.VersionOf = movieCredit.Id