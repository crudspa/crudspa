/*
merge into [Framework].[ExportProcedure] as Target
using ( values
-- todo: Add values
) as Source
    (Id, ProcedureName)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.ProcedureName = Source.ProcedureName

when not matched by target then
insert (Id, ProcedureName)
values (Id, ProcedureName)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/