merge into [Samples].[Tag] as Target
using ( values
     ('e55a4706-4dbe-1570-3cd5-6500d8332b60', 'Baking',    0)
    ,('60608c94-6696-fe32-8b8c-325fd309a79d', 'Community', 1)
    ,('738f9774-36ac-916d-eb02-ae08048caf4a', 'Craft',     2)
    ,('b86c88a7-72c4-4b42-e3b4-ae7047e5bfcc', 'Curiosity', 3)
    ,('6d2470e5-c917-d702-9a4b-95f9c4c38c8a', 'Gardens',   4)
    ,('354cd596-1138-8945-bf67-44ce9542b086', 'Maps',      5)
    ,('8763b671-65a5-1aca-49d2-58b9b8e77906', 'Music',     6)
    ,('18d96756-4d60-3a3c-b5b7-4f7a482bdea9', 'Nature',    7)
    ,('376d6912-e998-802b-a4ea-26256e2b7459', 'Projects',  8)
    ,('e2e90533-7e62-e594-d5b4-a27c7726f5a0', 'Travel',    9)
    ,('44b451cc-7541-55d5-eef2-e6b6fdd2c54f', 'Weather',   10)
    ,('f6c621cb-9bd7-c368-dfac-efbe10d553fb', 'Weekend',   11)
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