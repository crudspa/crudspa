create view [Education].[School-Active] as

select school.Id as Id
    ,school.OrganizationId as OrganizationId
    ,school.DistrictId as DistrictId
    ,school.CommunityId as CommunityId
    ,school.AddressId as AddressId
    ,school.[Key] as [Key]
    ,school.ImportNum as ImportNum
    ,school.Treatment as Treatment
    ,school.ResearchGroupId as ResearchGroupId
from [Education].[School] school
where 1=1
    and school.IsDeleted = 0
    and school.VersionOf = school.Id