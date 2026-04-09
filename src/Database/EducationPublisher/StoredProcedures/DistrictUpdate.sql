create proc [EducationPublisher].[DistrictUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@StudentIdNumberLabel nvarchar(50)
    ,@AssessmentExplainer nvarchar(max)
    ,@AddressId uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,StudentIdNumberLabel = @StudentIdNumberLabel
    ,AssessmentExplainer = @AssessmentExplainer
    ,AddressId = @AddressId
from [Education].[District] baseTable
    inner join [Education].[District-Active] district on district.Id = baseTable.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where baseTable.Id = @Id
    and publisher.Id = @publisherId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction