create view [Framework].[ExportRun-Active] as

select exportRun.Id as Id
    ,exportRun.ProcedureId as ProcedureId
    ,exportRun.Run as Run
from [Framework].[ExportRun] exportRun
where 1=1
    and exportRun.IsDeleted = 0