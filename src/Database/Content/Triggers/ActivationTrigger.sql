create trigger [Content].[ActivationTrigger] on [Content].[Activation]
    for update
as

insert [Content].[Activation] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BatchId
    ,CampaignId
    ,OrganizationId
    ,Start
    ,Activated
    ,ActivatedBy
    ,StatusId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BatchId
    ,deleted.CampaignId
    ,deleted.OrganizationId
    ,deleted.Start
    ,deleted.Activated
    ,deleted.ActivatedBy
    ,deleted.StatusId
from deleted