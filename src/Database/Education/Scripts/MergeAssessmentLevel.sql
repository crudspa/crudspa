/*
merge into [Education].[AssessmentLevel] as Target
using ( values
-- todo: Add values
) as Source
    (Id, [Key], Name, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.[Key] = Source.[Key]
    ,Target.Name = Source.Name
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, [Key], Name, Ordinal)
values (Id, [Key], Name, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/