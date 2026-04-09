create proc [ContentDisplay].[BinderProgressSelect] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
) as

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if (@contactId is null)
begin
    set @contactId = '00000000-0000-0000-0000-000000000000'
end

;with BinderCompletedCte(BinderId, EventCount) as (
    select binderCompleted.BinderId, count(1) as BinderCompletedCount
    from [Content].[BinderCompleted-Active] binderCompleted
    where binderCompleted.BinderId = @BinderId
        and binderCompleted.ContactId = @contactId
    group by binderCompleted.BinderId
)

select
    @contactId as ContactId
    ,binder.Id as BinderId
    ,isnull(bindersCompleted.EventCount, 0) as TimesCompleted
from [Content].[Binder-Active] binder
    left join BinderCompletedCte bindersCompleted on binder.Id = bindersCompleted.BinderId
where bindersCompleted.EventCount is not null