create proc [EducationPublisher].[AssessmentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(100)
    ,@StatusId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@AvailableStart date
    ,@AvailableEnd date
    ,@CategoryId uniqueidentifier
    ,@ImageFileId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
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
    ,Name = @Name
    ,StatusId = @StatusId
    ,GradeId = @GradeId
    ,AvailableStart = @AvailableStart
    ,AvailableEnd = @AvailableEnd
    ,CategoryId = @CategoryId
    ,ImageFileId = @ImageFileId
from [Education].[Assessment] baseTable
    inner join [Education].[Assessment-Active] assessment on assessment.Id = baseTable.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction