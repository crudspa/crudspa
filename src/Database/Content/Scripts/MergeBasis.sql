merge into [Content].[Basis] as Target
using ( values
     ('219250a4-1630-40ae-86e2-889e4b4c3efd', 'Auto',       0)
    ,('790dde51-b6f9-4726-b159-05fae3103843', 'Fixed',      1)
    ,('37fa27eb-52af-4b84-b4d1-b7f2d65dd9e9', 'Percentage', 2)
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