create proc [EducationPublisher].[ReadParagraphInsert] (
     @SessionId uniqueidentifier
    ,@ReadPartId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Education].[ReadParagraph-Active] where ReadPartId = @ReadPartId)

insert [Education].[ReadParagraph] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ReadPartId
    ,Text
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ReadPartId
    ,@Text
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[ReadParagraph-Active] readParagraph
        inner join [Education].[ReadPart-Active] readPart on readParagraph.ReadPartId = readPart.Id
        inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where readParagraph.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction