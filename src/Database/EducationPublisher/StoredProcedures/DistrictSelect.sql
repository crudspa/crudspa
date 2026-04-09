create proc [EducationPublisher].[DistrictSelect] (
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
     district.Id
    ,district.StudentIdNumberLabel
    ,district.AssessmentExplainer
    ,district.OrganizationId
    ,district.AddressId
    ,(select count(1) from [Education].[DistrictContact-Active] where DistrictId = district.Id) as DistrictContactCount
    ,(select count(1) from [Education].[Community-Active] where DistrictId = district.Id) as CommunityCount
    ,(select count(1) from [Education].[School-Active] where DistrictId = district.Id) as SchoolCount
from [Education].[District-Active] district
    left join [Framework].[UsaPostal-Active] address on district.AddressId = address.Id
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where district.Id = @Id
    and publisher.Id = @publisherId