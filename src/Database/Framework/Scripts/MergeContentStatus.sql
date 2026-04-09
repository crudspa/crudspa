merge into [Framework].[ContentStatus] as Target
using ( values
     ('3d73efa4-302d-47b9-a956-4fb4577e0c54', 'Draft',    0)
    ,('0296c1f0-7d72-42d3-b7c2-377f077e7b9c', 'Complete', 1)
    ,('2edbede5-e40e-4a7c-aba3-b369d127f629', 'Retired',  2)
) as Source
    (Id, Name, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, Ordinal)
values (Id, Name, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;