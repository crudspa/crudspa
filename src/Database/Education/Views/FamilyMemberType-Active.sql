create view [Education].[FamilyMemberType-Active] as

select familyMemberType.Id as Id
    ,familyMemberType.Name as Name
    ,familyMemberType.Ordinal as Ordinal
from [Education].[FamilyMemberType] familyMemberType
where 1=1
    and familyMemberType.IsDeleted = 0