create proc [ContentMessaging].[SmsMessageSelectWhereForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@OccurredStart datetimeoffset(7)
    ,@OccurredEnd datetimeoffset(7)
    ,@Direction int
    ,@LatestForConversation bit = 0
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with FilteredSmsMessageCte
as (
    select
         smsMessage.Id
        ,smsMessage.Occurred
        ,smsMessage.Status
        ,smsMessage.Direction
        ,case
            when smsMessage.ContactPhoneId is not null then convert(nvarchar(36), smsMessage.ContactPhoneId)
            else concat(
                 isnull(smsMessage.SmsChannelKey, N'')
                ,N'|'
                ,isnull(case when smsMessage.Direction = 0 then smsMessage.FromNumber else smsMessage.ToNumber end, N'')
                ,N'|'
                ,isnull(case when smsMessage.Direction = 0 then smsMessage.ToNumber else smsMessage.FromNumber end, N'')
            )
        end as ConversationKey
    from [Content].[SmsMessage-Active] smsMessage
        left join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
        left join [Content].[Sms-Active] sms on smsMessage.SmsId = sms.Id
        left join [Content].[Membership-Active] smsMembership on sms.MembershipId = smsMembership.Id
        left join [Content].[Member-Active] member on smsMessage.MemberId = member.Id
        left join [Content].[Membership-Active] memberMembership on member.MembershipId = memberMembership.Id
        left join [Framework].[Contact-Active] contact on smsMessage.ContactId = contact.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](
            @SessionId,
            coalesce(membership.PortalId, smsMembership.PortalId, memberMembership.PortalId),
            coalesce(membership.OrganizationId, smsMembership.OrganizationId, memberMembership.OrganizationId)
        )
    where 1 = 1
        and (
            membership.PortalId = @PortalId
            or smsMembership.PortalId = @PortalId
            or memberMembership.PortalId = @PortalId
        )
        and (@Direction is null or smsMessage.Direction = @Direction)
        and (@SearchText is null
            or smsMessage.Body like '%' + @SearchText + '%'
            or smsMessage.FromNumber like '%' + @SearchText + '%'
            or smsMessage.ToNumber like '%' + @SearchText + '%'
            or smsMessage.ProviderMessageId like '%' + @SearchText + '%'
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
        )
        and (@OccurredStart is null or smsMessage.Occurred >= @OccurredStart)
        and (@OccurredEnd is null or smsMessage.Occurred < @OccurredEnd)
),
ConversationSmsMessageCte
as (
    select
         filtered.Id
        ,filtered.Occurred
        ,filtered.Status
        ,filtered.Direction
        ,row_number() over (
            partition by filtered.ConversationKey
            order by filtered.Occurred desc, filtered.Id desc
        ) as ConversationRowNumber
    from FilteredSmsMessageCte filtered
),
ResultSmsMessageCte
as (
    select
         conversation.Id
        ,conversation.Occurred
        ,conversation.Status
        ,conversation.Direction
    from ConversationSmsMessageCte conversation
    where @LatestForConversation = 0
        or conversation.ConversationRowNumber = 1
),
SmsMessageCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Occurred' and @SortAscending = 1)
                    then result.Occurred
                end asc,
                case when (@SortField = 'Occurred' and @SortAscending = 0)
                    then result.Occurred
                end desc,
                case when (@SortField = 'Status' and @SortAscending = 1)
                    then result.Status
                end asc,
                case when (@SortField = 'Status' and @SortAscending = 0)
                    then result.Status
                end desc,
                case when (@SortField = 'Direction' and @SortAscending = 1)
                    then result.Direction
                end asc,
                case when (@SortField = 'Direction' and @SortAscending = 0)
                    then result.Direction
                end desc,
                case when (@SortAscending = 1)
                    then result.Id
                end asc,
                case when (@SortAscending = 0)
                    then result.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,result.Id
    from ResultSmsMessageCte result
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsMessage.Id
    ,smsMessage.MembershipId
    ,coalesce(membership.PortalId, smsMembership.PortalId, memberMembership.PortalId) as PortalId
    ,smsMessage.SmsId
    ,smsMessage.SmsChannelKey
    ,smsMessage.MemberId
    ,smsMessage.Body
    ,smsMessage.Direction
    ,smsMessage.Occurred
    ,smsMessage.FromNumber
    ,smsMessage.ToNumber
    ,smsMessage.Status
    ,smsMessage.ProviderMessageId
    ,smsMessage.Provider
    ,smsMessage.ApiResponse
    ,smsMessage.ContactPhoneId
    ,smsMessage.ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
from SmsMessageCte cte
    inner join [Content].[SmsMessage-Active] smsMessage on cte.Id = smsMessage.Id
    left join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
    left join [Content].[Sms-Active] sms on smsMessage.SmsId = sms.Id
    left join [Content].[Membership-Active] smsMembership on sms.MembershipId = smsMembership.Id
    left join [Content].[Member-Active] member on smsMessage.MemberId = member.Id
    left join [Content].[Membership-Active] memberMembership on member.MembershipId = memberMembership.Id
    left join [Framework].[Contact-Active] contact on smsMessage.ContactId = contact.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](
        @SessionId,
        coalesce(membership.PortalId, smsMembership.PortalId, memberMembership.PortalId),
        coalesce(membership.OrganizationId, smsMembership.OrganizationId, memberMembership.OrganizationId)
    )
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)