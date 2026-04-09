create proc [SamplesCatalog].[MovieCreditSelectForMovie] (
     @SessionId uniqueidentifier
    ,@MovieId uniqueidentifier
) as

set nocount on

select
     movieCredit.Id
    ,movieCredit.Name
    ,movieCredit.Part
    ,movieCredit.Billing
    ,movieCredit.MovieId
    ,movieCredit.Headliner
    ,movieCredit.Ordinal
from [Samples].[MovieCredit-Active] movieCredit
    inner join [Samples].[Movie-Active] movie on movieCredit.MovieId = movie.Id
where movieCredit.MovieId = @MovieId