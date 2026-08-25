merge into [Content].[MessageType] as Target
using ( values
     ('893356f5-0baa-4b66-8b72-798285c6a4db', 'Email', 0)
    ,('017e2c80-91ab-4090-bf7a-fe670ca4b180', 'SMS',   1)
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