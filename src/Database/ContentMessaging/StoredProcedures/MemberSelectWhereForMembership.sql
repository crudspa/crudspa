create proc [ContentMessaging].[MemberSelectWhereForMembership] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with MemberCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'First' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'First' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'Last' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'First' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'First' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'Last' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'Last' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortAscending = 1)
                    then member.Id
                end asc,
                case when (@SortAscending = 0)
                    then member.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,member.Id
    from [Content].[Member-Active] member
        inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
        inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where 1 = 1
        and member.MembershipId = @MembershipId
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,member.Id
    ,member.MembershipId
    ,member.Status
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
from MemberCte cte
    inner join [Content].[Member-Active] member on cte.Id = member.Id
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)