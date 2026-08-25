create proc [ContentMessaging].[MessageSelectWhereForMembership] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with MessageCte
as (
    select
        row_number() over (order by message.Id asc) as RowNumber
        ,count(*) over () as TotalCount
        ,message.Id
    from [Content].[Message-Active] message
        inner join [Content].[Activation-Active] activation on message.ActivationId = activation.Id
        inner join [Content].[Membership-Active] membership on message.MembershipId = membership.Id
        inner join [Content].[Stage-Active] stage on message.StageId = stage.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where 1 = 1
        and message.MembershipId = @MembershipId

        and (@SearchText is null
            or 1=1
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,message.Id
    ,message.MembershipId
    ,message.StageId
    ,stage.Name as StageName
    ,message.ActivationId
    ,message.EmailId
    ,message.SmsId
from MessageCte cte
    inner join [Content].[Message-Active] message on cte.Id = message.Id
    inner join [Content].[Activation-Active] activation on message.ActivationId = activation.Id
    inner join [Content].[Membership-Active] membership on message.MembershipId = membership.Id
    inner join [Content].[Stage-Active] stage on message.StageId = stage.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)