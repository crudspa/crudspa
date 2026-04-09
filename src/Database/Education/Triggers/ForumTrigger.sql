create trigger [Education].[ForumTrigger] on [Education].[Forum]
    for update
as

insert [Education].[Forum] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Name
    ,Description
    ,BodyTemplateId
    ,Pinned
    ,DistrictId
    ,SchoolId
    ,InnovatorsOnly
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Name
    ,deleted.Description
    ,deleted.BodyTemplateId
    ,deleted.Pinned
    ,deleted.DistrictId
    ,deleted.SchoolId
    ,deleted.InnovatorsOnly
from deleted