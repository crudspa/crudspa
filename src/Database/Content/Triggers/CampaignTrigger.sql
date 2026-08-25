create trigger [Content].[CampaignTrigger] on [Content].[Campaign]
    for update
as

insert [Content].[Campaign] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Name
    ,Description
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.Name
    ,deleted.Description
from deleted