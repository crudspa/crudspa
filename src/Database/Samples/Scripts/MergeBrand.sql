merge into [Samples].[Brand] as Target
using ( values
     ('4de30885-b095-6e3b-fb79-b6ae627e9e87', 'Daybreak Goods',    0)
    ,('a3d88125-7e1b-4bec-d50b-486c8ca6a345', 'North Loop Basics', 1)
    ,('55ef9965-ff27-ff43-3a6f-40597b295593', 'Harbor Thread',     2)
    ,('675dea84-479b-4a42-15ab-01f58a35de31', 'Pine and Pixel',    3)
    ,('1da612e9-c6c7-9ebf-1d37-8ed0e5d71c30', 'Common Kite',       4)
    ,('4c0061da-476a-a5e7-524b-0f0f8d297c4d', 'Maple Circuit',     5)
    ,('99ae651a-d5fb-6c85-d643-71854ab12c00', 'Field Note Supply', 6)
    ,('6bdda3dd-4a15-25a4-2dd6-e1b658953b13', 'Weekender Stitch',  7)
    ,('62702f85-cd92-8110-5315-190c1317c542', 'Bright Mile',       8)
    ,('7cc0bb10-465e-65a5-d1a1-de084cfa4d95', 'Studio Sprout',     9)
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