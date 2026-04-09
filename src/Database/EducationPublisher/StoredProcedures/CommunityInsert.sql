create proc [EducationPublisher].[CommunityInsert] (
     @SessionId uniqueidentifier
    ,@DistrictId uniqueidentifier
    ,@Name nvarchar(100)
    ,@Id uniqueidentifier output
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[Community] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,DistrictId
    ,Name
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@DistrictId
    ,@Name
)

if not exists (
    select 1
    from [Education].[Community-Active] community
        inner join [Education].[District-Active] district on community.DistrictId = district.Id
    where community.Id = @Id
        and district.PublisherId = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction