create proc [ContentMessaging].[SmsEventSelectWhereForSmsMessage] (
     @SessionId uniqueidentifier
    ,@SmsMessageId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with SmsEventCte
as (
    select
        row_number() over (order by smsEvent.Id asc) as RowNumber
        ,count(*) over () as TotalCount
        ,smsEvent.Id
    from [Content].[SmsEvent-Active] smsEvent
        inner join [Content].[SmsMessage-Active] smsMessage on smsEvent.SmsMessageId = smsMessage.Id
        inner join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where 1 = 1
        and smsEvent.SmsMessageId = @SmsMessageId

        and (@SearchText is null
            or 1=1
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsEvent.Id
    ,smsEvent.SmsMessageId
    ,smsEvent.ProviderMessageId
    ,smsEvent.Provider
    ,smsEvent.Type
    ,smsEvent.ProviderStatus
    ,smsEvent.Received
    ,smsEvent.Status
from SmsEventCte cte
    inner join [Content].[SmsEvent-Active] smsEvent on cte.Id = smsEvent.Id
    inner join [Content].[SmsMessage-Active] smsMessage on smsEvent.SmsMessageId = smsMessage.Id
    inner join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)