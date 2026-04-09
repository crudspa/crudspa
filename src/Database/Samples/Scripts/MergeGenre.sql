merge into [Samples].[Genre] as Target
using ( values
     ('94669580-905f-4f42-8ace-84788edf3547', 'Adventure',     0)
    ,('f653faa8-360b-b11d-cbf4-ca652e2c0d5d', 'Community',     1)
    ,('54243956-09d7-4722-c469-79709dd56eed', 'Cozy Mystery',  2)
    ,('097c00e3-88ae-5769-70df-3836e601416a', 'Drama',         3)
    ,('4cdce4ed-64fe-b4d4-89b6-23da031b6cbf', 'Fantasy',       4)
    ,('a0f8c4c5-5e86-903e-d12b-22f5680b6031', 'History',       5)
    ,('1bc55799-e7da-d2d4-5a81-62f463f168bf', 'Humor',         6)
    ,('916a40c5-a6f2-efcc-c842-3ef23a83da6f', 'Nature',        7)
    ,('b47cb446-13e8-7539-6497-ce6a6a2c7030', 'Practical',     8)
    ,('6e582712-8e61-eff0-5726-4f5c55244de6', 'Science',       9)
    ,('409ac709-8a84-29d7-4460-bd18651b4eba', 'Slice of Life', 10)
    ,('4fe095ea-76f6-eb94-24b6-5f2477d5d08b', 'Travel',        11)
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