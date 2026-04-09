create proc [SamplesComposer].[ComposerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[Composer] baseTable
    inner join [Samples].[Composer-Active] composer on composer.Id = baseTable.Id

where baseTable.Id = @Id
    and composer.OrganizationId = (
        select top 1 userTable.OrganizationId
        from [Framework].[Session-Active] session
            inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        where session.Id = @SessionId
            and session.Ended is null
    )

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction