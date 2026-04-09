create proc [EducationPublisher].[DistrictInsert] (
     @SessionId uniqueidentifier
    ,@StudentIdNumberLabel nvarchar(50)
    ,@AssessmentExplainer nvarchar(max)
    ,@OrganizationId uniqueidentifier
    ,@AddressId uniqueidentifier
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

insert [Education].[District] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,StudentIdNumberLabel
    ,AssessmentExplainer
    ,OrganizationId
    ,AddressId
    ,PublisherId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@StudentIdNumberLabel
    ,@AssessmentExplainer
    ,@OrganizationId
    ,@AddressId
    ,@publisherId
)

if not exists (
    select 1
    from [Education].[District-Active] district
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where district.Id = @Id
        and publisher.Id = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction