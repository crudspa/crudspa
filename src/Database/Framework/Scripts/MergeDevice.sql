merge into [Framework].[Device] as Target
using ( values
     ('e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', 'Local Process')
) as Source
    (Id, Name)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name

when not matched by target then
insert (Id, Name)
values (Id, Name)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;