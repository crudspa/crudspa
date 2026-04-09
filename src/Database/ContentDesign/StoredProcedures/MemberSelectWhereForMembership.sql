create proc [ContentDesign].[MemberSelectWhereForMembership] (
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

create table #MemberPage (
     RowNumber int not null
    ,TotalCount int not null
    ,MemberId uniqueidentifier not null
)

;with MemberCte as (
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
        ,member.Id as MemberId
    from [Content].[Member-Active] member
        inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
        inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
        outer apply (
            select top (1) email.Email
            from [Framework].[ContactEmail-Active] email
            where email.ContactId = contact.Id
            order by email.Ordinal
        ) contactEmail
    where 1 = 1
        and member.MembershipId = @MembershipId
        and organization.Id = @organizationId
        and (
            @SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or contactEmail.Email like '%' + @SearchText + '%'
        )
)
insert into #MemberPage (
     RowNumber
    ,TotalCount
    ,MemberId
)
select
     cte.RowNumber
    ,cte.TotalCount
    ,cte.MemberId
from MemberCte cte
where cte.RowNumber >= @firstRecord
    and cte.RowNumber <= @lastRecord
option (recompile)

select
     page.RowNumber
    ,page.TotalCount
    ,member.Id
    ,member.MembershipId
    ,member.Status
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,contactEmail.Email as ContactEmailEmail
from #MemberPage page
    inner join [Content].[Member-Active] member on page.MemberId = member.Id
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    outer apply (
        select top (1) email.Email
        from [Framework].[ContactEmail-Active] email
        where email.ContactId = contact.Id
        order by email.Ordinal
    ) contactEmail
order by page.RowNumber asc
option (recompile)

select
     tokenValue.Id
    ,tokenValue.TokenId
    ,tokenValue.ContactId
    ,tokenValue.Value
    ,token.[Key] as TokenKey
from #MemberPage page
    inner join [Content].[Member-Active] member on page.MemberId = member.Id
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    inner join [Content].[TokenValue-Active] tokenValue on tokenValue.ContactId = contact.Id
    inner join [Content].[Token-Active] token on tokenValue.TokenId = token.Id and member.MembershipId = token.MembershipId
order by
    page.RowNumber asc
    ,tokenValue.ContactId asc
    ,token.Ordinal
option (recompile)