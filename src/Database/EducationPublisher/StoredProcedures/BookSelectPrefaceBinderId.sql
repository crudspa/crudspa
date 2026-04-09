create proc [EducationPublisher].[BookSelectPrefaceBinderId] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
    book.Id
    ,book.PrefaceBinderId
from [Education].[Book-Active] book
where book.Id = @Id
    and book.OwnerId = @organizationId