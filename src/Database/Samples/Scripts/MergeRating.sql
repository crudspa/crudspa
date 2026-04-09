merge into [Samples].[Rating] as Target
using ( values
     ('e90a3536-3193-bd00-13df-5415e4d261f7', 'General',    0)
    ,('954ac1e1-4166-0a4e-f627-b1d2b3baa7d3', 'Family',     1)
    ,('3fba068d-56f9-95a6-02fa-5ed0383ac501', 'Guided 10+', 2)
    ,('e3c61f12-1f1c-99a9-98a6-c41b630d89d8', 'Guided 13+', 3)
    ,('bf8174e2-6d1d-863f-d4ec-6f075dd53802', 'Teen',       4)
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