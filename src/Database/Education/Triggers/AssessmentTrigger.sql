create trigger [Education].[AssessmentTrigger] on [Education].[Assessment]
    for update
as

insert [Education].[Assessment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OwnerId
    ,Name
    ,StatusId
    ,AvailableStart
    ,AvailableEnd
    ,GradeId
    ,CategoryId
    ,ImageFileId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OwnerId
    ,deleted.Name
    ,deleted.StatusId
    ,deleted.AvailableStart
    ,deleted.AvailableEnd
    ,deleted.GradeId
    ,deleted.CategoryId
    ,deleted.ImageFileId
from deleted