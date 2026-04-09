merge into [Content].[AlignContent] as Target
using ( values
     ('6514cd8a-e70f-4146-ae98-d2d189f9d35d', 'Around',  0)
    ,('adcec015-1ded-4967-b17a-575077aa951e', 'Between', 1)
    ,('1287a3a5-0fa4-4203-99ff-0c77d46270c0', 'Center',  2)
    ,('862280df-b103-424e-9652-b4b1e28c02d0', 'End',     3)
    ,('c17a7006-9045-4694-b10c-b6d41de59db5', 'Evenly',  4)
    ,('0ee37e59-45d9-4919-b8fc-d29477be8160', 'Normal',  5)
    ,('c05f8e57-be1a-449d-b99d-10e1b2a7c754', 'Start',   6)
    ,('d3446ce2-ef28-4dbd-9d78-65b540c3d34a', 'Stretch', 7)
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