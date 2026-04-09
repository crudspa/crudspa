create proc [EducationDistrict].[SchoolSelectSelectionsByCommunity] (
     @CommunityId uniqueidentifier
) as

declare @districtId uniqueidentifier = (select top 1 DistrictId from [Education].[Community-Active] where Id = @CommunityId)

select distinct
     @CommunityId as RootId
    ,school.Id as Id
    ,organization.[Name] as Name
    ,convert(bit, case when school.CommunityId = @CommunityId then 1 else 0 end) as Selected
from [Education].[School-Active] school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where school.DistrictId = @districtId
order by organization.[Name]