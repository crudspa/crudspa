merge into [Samples].[Color] as Target
using ( values
     ('6c56b974-f606-53ed-77f7-517ace1c7703', 'Cloud White',   0)
    ,('8a556412-792d-b91c-6b9a-57b847267944', 'Heather Gray',  1)
    ,('87a436d4-219d-64bb-d8ac-4872a724473f', 'Harbor Blue',   2)
    ,('003f602d-e04a-6799-87ba-e51184c9f43c', 'Pine Green',    3)
    ,('5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'Sunrise Peach', 4)
    ,('db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'Berry Fizz',    5)
    ,('26468024-3db4-41e9-7631-f5d55e316a6c', 'Sandstone',     6)
    ,('292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'Terracotta',    7)
    ,('0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'Midnight Ink',  8)
    ,('09dca023-98e9-0aab-c5ca-fadc47b51f45', 'Moss Green',    9)
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