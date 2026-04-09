merge into [Content].[Wrap] as Target
using ( values
     ('f0587ebb-68d8-4eaa-ae8c-66ba606d0445', 'Wrap',         0)
    ,('26c7b615-6152-4650-904f-03da610a0aae', 'No Wrap',      1)
    ,('80006150-6981-41ad-87aa-6ded2cd52ba8', 'Wrap Reverse', 2)
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