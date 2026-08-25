merge into [Content].[ActivationStatus] as Target
using ( values
     ('a9b01001-7c15-4fb8-b25f-9f5629031001', 'Draft',           0)
    ,('a9b01002-7c15-4fb8-b25f-9f5629031002', 'Scheduled',       1)
    ,('a9b01003-7c15-4fb8-b25f-9f5629031003', 'Active',          2)
    ,('a9b01004-7c15-4fb8-b25f-9f5629031004', 'Completed',       3)
    ,('a9b01005-7c15-4fb8-b25f-9f5629031005', 'Needs Attention', 4)
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