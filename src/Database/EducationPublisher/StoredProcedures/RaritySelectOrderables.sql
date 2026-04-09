create proc [EducationPublisher].[RaritySelectOrderables] as

set nocount on
select
     rarity.Id
    ,rarity.Name as Name
    ,rarity.Ordinal
from [Education].[Rarity-Active] rarity
order by rarity.Ordinal