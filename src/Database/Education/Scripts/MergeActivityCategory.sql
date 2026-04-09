/*
merge into [Education].[ActivityCategory] as Target
using ( values
-- todo: Add values
) as Source
    (Id, [Key], Name)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.[Key] = Source.[Key]
    ,Target.Name = Source.Name

when not matched by target then
insert (Id, [Key], Name)
values (Id, [Key], Name)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/