create proc [ContentMessaging].[SmsPreferenceSelectWhereForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@StatusChangedStart datetimeoffset(7)
    ,@StatusChangedEnd datetimeoffset(7)
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with SmsPreferenceCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Number' and @SortAscending = 1)
                    then smsPreference.Number
                end asc,
                case when (@SortField = 'Number' and @SortAscending = 0)
                    then smsPreference.Number
                end desc,
                case when (@SortField = 'Status Changed' and @SortAscending = 1)
                    then smsPreference.StatusChanged
                end asc,
                case when (@SortField = 'Status Changed' and @SortAscending = 0)
                    then smsPreference.StatusChanged
                end desc,
                case when (@SortField = 'Number' and @SortAscending = 1)
                    then smsPreference.StatusChanged
                end asc,
                case when (@SortField = 'Number' and @SortAscending = 0)
                    then smsPreference.StatusChanged
                end desc,
                case when (@SortField = 'Status Changed' and @SortAscending = 1)
                    then smsPreference.Number
                end asc,
                case when (@SortField = 'Status Changed' and @SortAscending = 0)
                    then smsPreference.Number
                end desc,
                case when (@SortAscending = 1)
                    then smsPreference.Id
                end asc,
                case when (@SortAscending = 0)
                    then smsPreference.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,smsPreference.Id
    from [Content].[SmsPreference-Active] smsPreference
        left join [Framework].[Contact-Active] contact on smsPreference.ContactId = contact.Id
        left join [Framework].[ContactPhone-Active] contactPhone on smsPreference.ContactPhoneId = contactPhone.Id
        inner join [Framework].[Portal-Active] portal on smsPreference.PortalId = portal.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, smsPreference.PortalId, smsPreference.OrganizationId)
    where 1 = 1
        and smsPreference.PortalId = @PortalId

        and (@SearchText is null
            or smsPreference.Number like '%' + @SearchText + '%'
        )
        and (@StatusChangedStart is null or smsPreference.StatusChanged >= @StatusChangedStart)
        and (@StatusChangedEnd is null or smsPreference.StatusChanged < @StatusChangedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,smsPreference.Id
    ,smsPreference.PortalId
    ,smsPreference.OrganizationId
    ,smsPreference.Number
    ,smsPreference.Status
    ,smsPreference.Source
    ,smsPreference.StatusChanged
    ,smsPreference.Notes
from SmsPreferenceCte cte
    inner join [Content].[SmsPreference-Active] smsPreference on cte.Id = smsPreference.Id
    left join [Framework].[Contact-Active] contact on smsPreference.ContactId = contact.Id
    left join [Framework].[ContactPhone-Active] contactPhone on smsPreference.ContactPhoneId = contactPhone.Id
    inner join [Framework].[Portal-Active] portal on smsPreference.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, smsPreference.PortalId, smsPreference.OrganizationId)
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)