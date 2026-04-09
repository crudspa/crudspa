create trigger [Education].[FamilyTrigger] on [Education].[Family]
    for update
as

insert [Education].[Family] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,SchoolId
    ,ImportNum
    ,SurveyComplete
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.SchoolId
    ,deleted.ImportNum
    ,deleted.SurveyComplete
from deleted