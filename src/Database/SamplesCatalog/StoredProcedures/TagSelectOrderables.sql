create proc [SamplesCatalog].[TagSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     tag.Id
    ,tag.Name as Name
    ,tag.Ordinal
from [Samples].[Tag-Active] tag
order by tag.Ordinal