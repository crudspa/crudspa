create proc [ContentDesign].[ForumUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
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

if exists (
    select 1
    from @Orderables orderable
        left join (
            select forum.Id, forum.PortalId
            from [Content].[Forum-Active] forum
                inner join [Framework].[Portal-Active] portal on forum.PortalId = portal.Id
            where portal.OwnerId = @organizationId
        ) authorizedForum on authorizedForum.Id = orderable.Id
    where authorizedForum.Id is null
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (
    select count(distinct forum.PortalId)
    from [Content].[Forum-Active] forum
        inner join @Orderables orderable on orderable.Id = forum.Id
) > 1
begin
    rollback transaction
    raiserror('Forums must belong to the same portal', 16, 1)
    return
end

update forum
set
     forum.Ordinal = orderable.Ordinal
    ,forum.Updated = @now
    ,forum.UpdatedBy = @SessionId
from [Content].[Forum] forum
    inner join [Content].[Forum-Active] activeForum on activeForum.Id = forum.Id
    inner join @Orderables orderable on orderable.Id = activeForum.Id
where forum.Ordinal != orderable.Ordinal

commit transaction