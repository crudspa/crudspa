create proc [EducationPublisher].[AssessmentInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(100)
    ,@StatusId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@AvailableStart date
    ,@AvailableEnd date
    ,@CategoryId uniqueidentifier
    ,@ImageFileId uniqueidentifier
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

insert [Education].[Assessment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,StatusId
    ,GradeId
    ,AvailableStart
    ,AvailableEnd
    ,CategoryId
    ,ImageFileId
    ,OwnerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@StatusId
    ,@GradeId
    ,@AvailableStart
    ,@AvailableEnd
    ,@CategoryId
    ,@ImageFileId
    ,@organizationId
)

if not exists (
    select 1
    from [Education].[Assessment-Active] assessment
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where assessment.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction