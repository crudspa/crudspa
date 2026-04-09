merge into [Content].[AlignItems] as Target
using ( values
     ('2ecc6246-7022-482a-8366-b337aeef6bfd', 'Baseline', 0)
    ,('e2a5a254-c698-4bbe-9ac3-a1d75598aeb2', 'Center',   1)
    ,('5575042d-ab8d-4b42-b56f-408d85d3c1af', 'End',      2)
    ,('cc9219f1-3e61-4e44-805b-e4fdc0d8b8b5', 'Normal',   3)
    ,('2ce851f6-825c-4240-843b-fc55cbd77b63', 'Start',    4)
    ,('ce713d80-557e-4983-9ef7-a016c6191177', 'Stretch',  5)
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