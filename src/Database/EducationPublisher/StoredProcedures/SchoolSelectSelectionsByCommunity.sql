create proc [EducationPublisher].[SchoolSelectSelectionsByCommunity] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @districtId uniqueidentifier = (
    select top 1 community.DistrictId
    from [Education].[Community-Active] community
        inner join [Education].[District-Active] district on community.DistrictId = district.Id
    where community.Id = @CommunityId
        and district.PublisherId = @publisherId
)

select distinct
     @CommunityId as RootId
    ,school.Id as Id
    ,organization.[Name] as Name
    ,convert(bit, case when school.CommunityId = @CommunityId then 1 else 0 end) as Selected
from [Education].[School-Active] school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where school.DistrictId = @districtId
order by organization.[Name]