create proc [ContentMessaging].[EmailSentSelectWhereForEmail] (
     @SessionId uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Statuses Framework.IntList readonly
    ,@ProcessedStart datetimeoffset(7)
    ,@ProcessedEnd datetimeoffset(7)
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
declare @statusesCount int = (select count(1) from @Statuses)

;with EmailSentCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Sent' and @SortAscending = 1)
                    then emailLog.Processed
                end asc,
                case when (@SortField = 'Sent' and @SortAscending = 0)
                    then emailLog.Processed
                end desc,
                case when (@SortField = 'Recipient' and @SortAscending = 1)
                    then emailLog.RecipientEmail
                end asc,
                case when (@SortField = 'Recipient' and @SortAscending = 0)
                    then emailLog.RecipientEmail
                end desc,
                case when (@SortField = 'Sent' and @SortAscending = 1)
                    then emailLog.RecipientEmail
                end asc,
                case when (@SortField = 'Sent' and @SortAscending = 0)
                    then emailLog.RecipientEmail
                end desc,
                case when (@SortAscending = 1)
                    then emailLog.Id
                end asc,
                case when (@SortAscending = 0)
                    then emailLog.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,emailLog.Id
    from [Content].[EmailLog-Active] emailLog
        inner join [Content].[Email-Active] email on emailLog.EmailId = email.Id
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
        left join [Framework].[Contact-Active] contact on emailLog.RecipientId = contact.Id
    where 1 = 1
        and emailLog.EmailId = @EmailId
        and (@SearchText is null
            or emailLog.RecipientEmail like '%' + @SearchText + '%'
            or emailLog.ApiResponse like '%' + @SearchText + '%'
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
        )
        and (@statusesCount = 0 or emailLog.Status in (select Id from @Statuses))
        and (@ProcessedStart is null or emailLog.Processed >= @ProcessedStart)
        and (@ProcessedEnd is null or emailLog.Processed < @ProcessedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,emailLog.Id
    ,emailLog.EmailId
    ,emailLog.RecipientId
    ,emailLog.RecipientEmail
    ,emailLog.Processed
    ,emailLog.Status
    ,emailLog.ApiResponse
    ,contact.FirstName as RecipientFirstName
    ,contact.LastName as RecipientLastName
from EmailSentCte cte
    inner join [Content].[EmailLog-Active] emailLog on cte.Id = emailLog.Id
    left join [Framework].[Contact-Active] contact on emailLog.RecipientId = contact.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)