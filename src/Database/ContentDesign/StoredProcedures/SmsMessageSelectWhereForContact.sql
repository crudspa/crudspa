create proc [ContentDesign].[SmsMessageSelectWhereForContact] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@OccurredStart datetimeoffset(7)
    ,@OccurredEnd datetimeoffset(7)
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with SmsMessageCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Occurred' and @SortAscending = 1)
                    then smsMessage.Occurred
                end asc,
                case when (@SortField = 'Occurred' and @SortAscending = 0)
                    then smsMessage.Occurred
                end desc,
                case when (@SortField = 'Status' and @SortAscending = 1)
                    then smsMessage.Status
                end asc,
                case when (@SortField = 'Status' and @SortAscending = 0)
                    then smsMessage.Status
                end desc,
                case when (@SortField = 'Occurred' and @SortAscending = 1)
                    then smsMessage.Status
                end asc,
                case when (@SortField = 'Occurred' and @SortAscending = 0)
                    then smsMessage.Status
                end desc,
                case when (@SortField = 'Status' and @SortAscending = 1)
                    then smsMessage.Occurred
                end asc,
                case when (@SortField = 'Status' and @SortAscending = 0)
                    then smsMessage.Occurred
                end desc,
                case when (@SortAscending = 1)
                    then smsMessage.Id
                end asc,
                case when (@SortAscending = 0)
                    then smsMessage.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,smsMessage.Id
    from [Content].[SmsMessage-Active] smsMessage
        left join [Framework].[Contact-Active] contact on smsMessage.ContactId = contact.Id
        left join [Framework].[ContactPhone-Active] contactPhone on smsMessage.ContactPhoneId = contactPhone.Id
        left join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
    where 1 = 1
        and smsMessage.ContactId = @ContactId

        and (@SearchText is null
            or smsMessage.Body like '%' + @SearchText + '%'
            or smsMessage.FromNumber like '%' + @SearchText + '%'
            or smsMessage.ToNumber like '%' + @SearchText + '%'
        )
        and (@OccurredStart is null or smsMessage.Occurred >= @OccurredStart)
        and (@OccurredEnd is null or smsMessage.Occurred < @OccurredEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsMessage.Id
    ,smsMessage.ContactId
    ,smsMessage.Body
    ,smsMessage.Direction
    ,smsMessage.Occurred
    ,smsMessage.FromNumber
    ,smsMessage.ToNumber
    ,smsMessage.Status
from SmsMessageCte cte
    inner join [Content].[SmsMessage-Active] smsMessage on cte.Id = smsMessage.Id
    left join [Framework].[Contact-Active] contact on smsMessage.ContactId = contact.Id
    left join [Framework].[ContactPhone-Active] contactPhone on smsMessage.ContactPhoneId = contactPhone.Id
    left join [Content].[Membership-Active] membership on smsMessage.MembershipId = membership.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)