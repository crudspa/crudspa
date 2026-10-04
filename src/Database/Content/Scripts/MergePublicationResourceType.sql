merge into [Content].[PublicationResourceType] as Target
using ( values
     ('12c00bf4-9ace-5792-a6d6-c86e7b92b3da', 'Research brief',   0)
    ,('ade43b28-8c99-5900-9230-8734e8d75706', 'Paper',            1)
    ,('c11c3e86-80bc-582a-bb7e-66a102d5250d', 'Registration',     2)
    ,('18d983da-4c75-5427-8695-6f61c59ba588', 'Replication data', 3)
    ,('5304b095-e607-5683-b5ab-b2d7469da056', 'Supplement',       4)
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