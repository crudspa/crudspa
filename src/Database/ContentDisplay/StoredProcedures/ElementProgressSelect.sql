create proc [ContentDisplay].[ElementProgressSelect] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
) as

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with ElementCompletedCte(ElementId, EventCount) as (
    select elementCompleted.ElementId, count(1) as ElementCompletedCount
    from [Content].[ElementCompleted-Active] elementCompleted
    where elementCompleted.ElementId = @ElementId
        and elementCompleted.ContactId = @contactId
    group by elementCompleted.ElementId
)

select
    @contactId as ContactId
    ,element.Id as ElementId
    ,isnull(elementsCompleted.EventCount, 0) as TimesCompleted
from [Content].[Element-Active] element
    left join ElementCompletedCte elementsCompleted on element.Id = elementsCompleted.ElementId
where elementsCompleted.EventCount is not null