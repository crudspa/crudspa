create proc [EducationPublisher].[SchoolSelectRoleNames] (
     @SessionId uniqueidentifier
    ,@SchoolId uniqueidentifier
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
     role.Id
    ,role.Name
from [Framework].[Role-Active] role
    inner join [Framework].[Organization-Active] organization on role.OrganizationId = organization.Id
    inner join [Education].[School-Active] school on school.OrganizationId = organization.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where 1 = 1
    and school.Id = @SchoolId
    and publisher.Id = @publisherId
order by role.Name