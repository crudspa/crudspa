create proc [ContentDesign].[SmsEventSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@ReceivedStart datetimeoffset(7)
    ,@ReceivedEnd datetimeoffset(7)
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with SmsEventCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Received' and @SortAscending = 1)
                    then smsEvent.Received
                end asc,
                case when (@SortField = 'Received' and @SortAscending = 0)
                    then smsEvent.Received
                end desc,
                case when (@SortField = 'Status' and @SortAscending = 1)
                    then smsEvent.Status
                end asc,
                case when (@SortField = 'Status' and @SortAscending = 0)
                    then smsEvent.Status
                end desc,
                case when (@SortField = 'Received' and @SortAscending = 1)
                    then smsEvent.Status
                end asc,
                case when (@SortField = 'Received' and @SortAscending = 0)
                    then smsEvent.Status
                end desc,
                case when (@SortField = 'Status' and @SortAscending = 1)
                    then smsEvent.Received
                end asc,
                case when (@SortField = 'Status' and @SortAscending = 0)
                    then smsEvent.Received
                end desc,
                case when (@SortAscending = 1)
                    then smsEvent.Id
                end asc,
                case when (@SortAscending = 0)
                    then smsEvent.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,smsEvent.Id
    from [Content].[SmsEvent-Active] smsEvent
        left join [Content].[SmsMessage-Active] smsMessage on smsEvent.SmsMessageId = smsMessage.Id
    where 1 = 1

        and (@SearchText is null
            or smsEvent.ProviderMessageId like '%' + @SearchText + '%'
            or smsEvent.ProviderStatus like '%' + @SearchText + '%'
        )
        and (@ReceivedStart is null or smsEvent.Received >= @ReceivedStart)
        and (@ReceivedEnd is null or smsEvent.Received < @ReceivedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsEvent.Id
    ,smsEvent.ProviderMessageId
    ,smsEvent.Provider
    ,smsEvent.Type
    ,smsEvent.ProviderStatus
    ,smsEvent.SignatureValid
    ,smsEvent.Received
    ,smsEvent.Processed
    ,smsEvent.Status
    ,smsEvent.ErrorMessage
from SmsEventCte cte
    inner join [Content].[SmsEvent-Active] smsEvent on cte.Id = smsEvent.Id
    left join [Content].[SmsMessage-Active] smsMessage on smsEvent.SmsMessageId = smsMessage.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)