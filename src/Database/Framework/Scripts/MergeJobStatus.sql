merge into [Framework].[JobStatus] as Target
using ( values
     ('5e2d54a0-5774-4cae-8391-0b6ac31d4f60', 'Pending',   'status-pending',   0)
    ,('28886325-475c-4d3e-9624-96e9c151775d', 'Running',   'status-running',   1)
    ,('81c1ccdb-cbf3-4a6a-845e-ca8839c17d2d', 'Completed', 'status-completed', 2)
    ,('c6416f41-8dc5-424d-a53b-04f13ad3568d', 'Failed',    'status-failed',    3)
    ,('3461ccbd-4ceb-4de4-94e5-0c6b3e36ae9d', 'Canceled',  'status-canceled',  4)
) as Source
    (Id, Name, CssClass, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.CssClass = Source.CssClass
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, CssClass, Ordinal)
values (Id, Name, CssClass, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;