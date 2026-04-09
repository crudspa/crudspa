create trigger [Education].[SchoolTrigger] on [Education].[School]
    for update
as

insert [Education].[School] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,DistrictId
    ,CommunityId
    ,AddressId
    ,[Key]
    ,ImportNum
    ,Treatment
    ,ResearchGroupId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.DistrictId
    ,deleted.CommunityId
    ,deleted.AddressId
    ,deleted.[Key]
    ,deleted.ImportNum
    ,deleted.Treatment
    ,deleted.ResearchGroupId
from deleted