create trigger [Education].[DistrictTrigger] on [Education].[District]
    for update
as

insert [Education].[District] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,PublisherId
    ,AddressId
    ,StudentIdNumberLabel
    ,AssessmentExplainer
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.PublisherId
    ,deleted.AddressId
    ,deleted.StudentIdNumberLabel
    ,deleted.AssessmentExplainer
from deleted