create proc [EducationDistrict].[CommunitySelectNames] (
     @SessionId uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
     community.Id
    ,community.Name
from [Education].[Community-Active] community
where community.DistrictId = @districtId
order by community.Name