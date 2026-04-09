;disable trigger [Framework].[DeviceJobTypeTrigger] on [Framework].[DeviceJobType];

declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset(7) = sysdatetimeoffset()

merge into [Framework].[DeviceJobType] as Target
using ( values
     ('6f2f0cf6-ea39-4890-8d2c-a46724143fe1', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', '3c03a379-d953-4823-8381-5d8772a42415')
    ,('49b9f1ab-6104-4d40-96fe-2e78626ed95b', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', '7707dafb-3c2f-4f2a-a526-d0e20a01de3c')
    ,('17ce17e1-ac3e-4a1b-9f20-72c3e9947c66', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', '84f36181-4bbf-413c-8732-58b61ae0ab5d')
    ,('ea72cea5-e797-4ee6-b78e-15eb26b970fa', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', 'bf7dfe75-2177-4103-806e-685555515942')
    ,('ad033ae0-e994-456d-b4a4-c2b1878ffe92', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', 'd17d15df-ba1d-4f29-ab74-c8c75e4ee111')
    ,('409ef101-9ca3-4dbe-87af-6661628d2c84', 'e187f9ae-f0f8-4a00-a8d5-481e0fb17e79', 'e2adc40c-46c5-409a-9396-1ca0a1e12d0c')
) as Source
    (Id, DeviceId, JobTypeId)
on Target.Id = Source.Id

when matched and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 0
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId
    ,Target.DeviceId = Source.DeviceId
    ,Target.JobTypeId = Source.JobTypeId

when not matched by target then
insert (Id, VersionOf, Updated, UpdatedBy, DeviceId, JobTypeId)
values (Id, Id, @now, @sessionId, DeviceId, JobTypeId)

when not matched by source and Target.IsDeleted = 0 and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 1
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId

;enable trigger [Framework].[DeviceJobTypeTrigger] on [Framework].[DeviceJobType];