create proc [ContentMessaging].[ActivationSelectWhereForOrganization] (
     @SessionId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with ActivationCte
as (
    select
        row_number() over (order by activation.Id asc) as RowNumber
        ,count(*) over () as TotalCount
        ,activation.Id
    from [Content].[Activation-Active] activation
        inner join [Content].[Campaign-Active] campaign on activation.CampaignId = campaign.Id
        inner join [Framework].[Organization-Active] organization on activation.OrganizationId = organization.Id
        cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
    where 1 = 1
        and activation.OrganizationId = @OrganizationId

        and (@SearchText is null
            or 1=1
        )
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,activation.Id
    ,activation.OrganizationId
    ,activation.CampaignId
    ,campaign.Name as CampaignName
    ,activation.BatchId
    ,activation.Start
    ,activation.Activated
    ,activation.ActivatedBy
from ActivationCte cte
    inner join [Content].[Activation-Active] activation on cte.Id = activation.Id
    inner join [Content].[Campaign-Active] campaign on activation.CampaignId = campaign.Id
    inner join [Framework].[Organization-Active] organization on activation.OrganizationId = organization.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)