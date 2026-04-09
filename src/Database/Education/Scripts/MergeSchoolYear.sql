/*
merge into [Education].[SchoolYear] as Target
using ( values
-- todo: Add values
) as Source
    (Id, Name, Starts, Ends)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.Starts = Source.Starts
    ,Target.Ends = Source.Ends

when not matched by target then
insert (Id, Name, Starts, Ends)
values (Id, Name, Starts, Ends)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/