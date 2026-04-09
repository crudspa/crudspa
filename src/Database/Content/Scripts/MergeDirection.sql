merge into [Content].[Direction] as Target
using ( values
     ('f16b220c-533d-4176-ac71-ec7278aa147f', 'Row',    0)
    ,('bc447d22-1090-48e8-86f8-65f2efbc38d7', 'Column', 1)
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