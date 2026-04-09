create proc [SamplesCatalog].[RatingSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     rating.Id
    ,rating.Name as Name
    ,rating.Ordinal
from [Samples].[Rating-Active] rating
order by rating.Ordinal