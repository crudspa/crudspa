create proc [ContentMessaging].[StageSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     stage.Id
    ,stage.CampaignId
    ,stage.Name
    ,stage.Offset
    ,stage.Anchor
    ,stage.WeekendAdjustment
    ,stage.SendTime
    ,stage.Ordinal
    ,(
        select count(*)
        from [Content].[Message-Active] message
        where message.StageId = stage.Id
            and message.ActivationId is null
    ) as MessageCount
from [Content].[Stage-Active] stage
    inner join [Content].[Campaign-Active] campaign on stage.CampaignId = campaign.Id
    cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
where stage.Id = @Id