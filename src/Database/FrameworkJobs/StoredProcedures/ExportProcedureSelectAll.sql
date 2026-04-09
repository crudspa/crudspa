create proc [FrameworkJobs].[ExportProcedureSelectAll] as

select
     exportProcedure.Id
    ,exportProcedure.ProcedureName
    ,lastRun.ExportRunLastRun
from [Framework].[ExportProcedure-Active] exportProcedure
    outer apply (
        select top 1
            exportRun.Run as ExportRunLastRun
        from [Framework].[ExportRun-Active] exportRun
        where exportRun.ProcedureId = exportProcedure.Id
        order by exportRun.Run desc
    ) lastRun
order by exportProcedure.ProcedureName