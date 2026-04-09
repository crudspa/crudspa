create proc [SamplesCatalog].[GenreSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     genre.Id
    ,genre.Name as Name
    ,genre.Ordinal
from [Samples].[Genre-Active] genre
order by genre.Ordinal