create function [EducationPublisher].[MyDistrictContacts] (
     @ContactId uniqueidentifier
)
returns @Contacts table (
     [Id] uniqueidentifier
    ,[Name] nvarchar(151)
    ,[OrganizationName] nvarchar(75)
)
as
begin

    declare @publisherId uniqueidentifier = (
        select top 1 publisher.Id
        from [Education].[Publisher-Active] publisher
            inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
            inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
            inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
            inner join [Framework].[Contact-Active] contact on userTable.ContactId = contact.Id
        where contact.Id = @ContactId
    )

    insert into @Contacts
    select distinct
         contact.Id
        ,trim(isnull(contact.FirstName, '') + ' ' + isnull(contact.LastName, '')) as Name
        ,organization.Name as OrganizationName
    from [Framework].[Contact-Active] contact
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.ContactId = contact.Id
        inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
        inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where publisher.Id = @publisherId

    return
end