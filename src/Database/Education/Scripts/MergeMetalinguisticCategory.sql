/*
merge into [Education].[MetalinguisticCategory] as Target
using ( values
-- todo: Add values
) as Source
    (Id, Name, [Key])
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.[Key] = Source.[Key]

when not matched by target then
insert (Id, Name, [Key])
values (Id, Name, [Key])

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/