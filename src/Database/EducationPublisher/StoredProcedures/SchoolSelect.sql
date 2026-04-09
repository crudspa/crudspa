create proc [EducationPublisher].[SchoolSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     school.Id
    ,school.DistrictId
    ,school.[Key]
    ,school.CommunityId
    ,community.Name as CommunityName
    ,school.Treatment
    ,school.OrganizationId
    ,school.AddressId
    ,(select count(1) from [Education].[SchoolContact-Active] where SchoolId = school.Id) as SchoolContactCount
from [Education].[School-Active] school
    left join [Framework].[UsaPostal-Active] address on school.AddressId = address.Id
    left join [Education].[Community-Active] community on school.CommunityId = community.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where school.Id = @Id
    and publisher.Id = @publisherId