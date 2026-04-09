create view [Education].[AudioGender-Active] as

select audioGender.Id as Id
    ,audioGender.Name as Name
    ,audioGender.Ordinal as Ordinal
from [Education].[AudioGender] audioGender
where 1=1
    and audioGender.IsDeleted = 0