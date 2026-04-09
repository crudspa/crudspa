merge into [Content].[JustifyContent] as Target
using ( values
     ('b70a2f94-2ef4-45e1-9196-9509585b9dec', 'Around',  0)
    ,('3f09c917-65d6-445d-9c77-28911a696ffb', 'Between', 1)
    ,('f32198b3-77a0-4877-aa12-8d10acc0ac07', 'Center',  2)
    ,('87bebfa4-4aed-42ae-a4de-1b37762d5858', 'End',     3)
    ,('fecd11cb-55c3-4df9-8d10-01bc1b565c8c', 'Evenly',  4)
    ,('f226c64f-1233-42c0-b40f-41688ca817bf', 'Normal',  5)
    ,('40a280a3-9498-49e0-80e6-11b923d9e86d', 'Start',   6)
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