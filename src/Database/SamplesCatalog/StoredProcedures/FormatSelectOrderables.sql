create proc [SamplesCatalog].[FormatSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     format.Id
    ,format.Name as Name
    ,format.Ordinal
from [Samples].[Format-Active] format
order by format.Ordinal