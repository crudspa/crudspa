create view [Framework].[ExportProcedure-Active] as

select exportProcedure.Id as Id
    ,exportProcedure.ProcedureName as ProcedureName
from [Framework].[ExportProcedure] exportProcedure
where 1=1
    and exportProcedure.IsDeleted = 0