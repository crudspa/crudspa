create proc [ContentDesign].[EmailSelectWhereForMembership] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@SendStart datetimeoffset(7)
    ,@SendEnd datetimeoffset(7)
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

;with EmailCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Send' and @SortAscending = 1)
                    then email.Send
                end asc,
                case when (@SortField = 'Send' and @SortAscending = 0)
                    then email.Send
                end desc,
                case when (@SortField = 'Subject' and @SortAscending = 1)
                    then email.Subject
                end asc,
                case when (@SortField = 'Subject' and @SortAscending = 0)
                    then email.Subject
                end desc,
                case when (@SortField = 'Send' and @SortAscending = 1)
                    then email.Subject
                end asc,
                case when (@SortField = 'Send' and @SortAscending = 0)
                    then email.Subject
                end desc,
                case when (@SortField = 'Subject' and @SortAscending = 1)
                    then email.Send
                end asc,
                case when (@SortField = 'Subject' and @SortAscending = 0)
                    then email.Send
                end desc,
                case when (@SortAscending = 1)
                    then email.Id
                end asc,
                case when (@SortAscending = 0)
                    then email.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,email.Id
    from [Content].[Email-Active] email
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
        left join [Content].[EmailTemplate-Active] template on email.TemplateId = template.Id
    where 1 = 1
        and email.MembershipId = @MembershipId
        and organization.Id = @organizationId
        and (@SearchText is null
            or email.Subject like '%' + @SearchText + '%'
            or email.Body like '%' + @SearchText + '%'
        )
        and (@SendStart is null or email.Send >= @SendStart)
        and (@SendEnd is null or email.Send < @SendEnd)
        and (@ProcessedStart is null or email.Processed >= @ProcessedStart)
        and (@ProcessedEnd is null or email.Processed < @ProcessedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,email.Id
    ,email.MembershipId
    ,email.Subject
    ,email.FromName
    ,email.FromEmail
    ,email.TemplateId
    ,template.Title as TemplateTitle
    ,email.Send
    ,email.Body
    ,email.Status
    ,email.Processed
from EmailCte cte
    inner join [Content].[Email-Active] email on cte.Id = email.Id
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    left join [Content].[EmailTemplate-Active] template on email.TemplateId = template.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)